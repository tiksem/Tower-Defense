using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;

public class Weapon : MonoBehaviour 
{
	public enum AttackType
	{
		NORMAL,
		SIEGE,
		PIERCING,
		DEATH,
		HOLY,
		FIRE,
		POISON,
		ICE,
		GROUND,
		WIND
	}
	
	[System.Serializable]
	public class BulletDefinition
	{
		public String name = "Untitled";
		
		public GameObject bullet;
	
		public float cooldown = 1.0f;
		public float range = 1000.0f;
		
		public int damage = 0;
		public AttackType attackType = AttackType.NORMAL;
		
		public bool ground = true;
		public bool air = true;
		
		public GameObject[] additionalEffects;
		public float[] effectsChances;
	}
	
	private static float UPDATE_TARGETS_DELAY = 0.5f;
	
	public bool attackingEnabled = true;
	public bool renderWhileAttackingOnly = false;
	public float renderingDuration = 0.5f;
	public float delayBeforeRender = 0.5f;
	
	public BulletDefinition[] attackTypes;
	
	private GameObject[] targets;
	private GameObject[] availibleTargets;
	private float[] lastAttackTimePerAttackType;
	
	private ParticleSystem particleSystem;
	private Renderer rendererComponent;
	private Tower tower;
	
	private bool shouldUpdateRenderingState = true;
	
	private int DistanceComparator(object a, object b)
	{		
		float distanceToA = GetDistanceTo(a);
		float distanceToB = GetDistanceTo(b);
		return distanceToA.CompareTo(distanceToB);
	}
	
	private void CheckBulletDefinitions()
	{
		
	}
	
	void Awake()
	{
		targets = new GameObject[attackTypes.Length];
		lastAttackTimePerAttackType = new float[attackTypes.Length];
		System.Array.Sort(attackTypes, (BulletDefinition a, BulletDefinition b) => -a.range.CompareTo(b.range));
		CheckBulletDefinitions();
	}
	
	private float GetEffectChance(BulletDefinition bulletDefinition, int index)
	{
		float[] effectsChances = bulletDefinition.effectsChances;
		if(index < effectsChances.Length)
		{
			return effectsChances[index];
		}
		else
		{
			return 1.0f;
		}
	}
	
	private float GetDistanceTo(object target)
	{
		float result;
		if(target is float)
		{
			result = (float)target;
		}
		else
		{
			result = Vector3.Distance(transform.position, ((GameObject)target).transform.position);
		}
		
		return result;
	}
	
	private bool CanAttackTarget(BulletDefinition attackType, Target target)
	{
		if(target.HasImmunity(attackType.attackType))
		{
			return false;
		}
		
		if(target.type == Target.Type.GROUND && attackType.ground)
		{
			return true;
		}
		
		if(target.type == Target.Type.AIR && attackType.air)
		{
			return true;
		}
		
		return false;
	}
	
	private GameObject[] GetAttackableTargets(BulletDefinition attackType)
	{
		return Collections.Filter(availibleTargets, (int index) =>
		{
			GameObject target = availibleTargets[index];
			Target targetComponent = target.GetComponent<Target>();
			return CanAttackTarget(attackType, targetComponent);
		});
	}
	
	private void ApplyInvisibileTargetsDetection(int lastIndex)
	{
		if(tower != null && tower.canSeeInvisibleUnits)
		{
			for(int i = 0; i < availibleTargets.Length; i++)
			{
				GameObject target = availibleTargets[i];
				Target targetComponent = target.GetComponent<Target>();
				targetComponent.SetVisible(i <= lastIndex);
			}
		}
		
		
	}
	
	private int GetFarthestVisibleTargetIndex(int lastIndex)
	{
		for(int i = lastIndex; i >= 0; i--)
		{
			GameObject target = availibleTargets[i];
			Target targetComponent = target.GetComponent<Target>();
			if(targetComponent.IsVisible())
			{
				return i;
			}
		}
		
		return -1;
	}
	
	private GameObject FindTargetForAttackType(BulletDefinition attackType)
	{
		GameObject[] attackableTargets = GetAttackableTargets(attackType);
		int index = Array.BinarySearch(attackableTargets, attackType.range, new AssemblyCSharp.ComparisonComparer<object>(DistanceComparator));
		if(index < 0)
		{
			index = -index - 2;
		}
		
		if(index < 0)
		{
			return null;
		}
		
		ApplyInvisibileTargetsDetection(index);
		index = GetFarthestVisibleTargetIndex(index);
		
		if(index < 0)
		{
			return null;
		}
		
		return availibleTargets[index];
	}
	
	private bool IsTargetValid(GameObject target, BulletDefinition attackType)
	{
		if(target == null)
		{
			return false;
		}
		
		return DistanceComparator(target, attackType.range) <= 0;
	}
	
	public static GameObject[] GetAvailibleTargets()
	{
		return GameObject.FindGameObjectsWithTag("Target");
	}
	
	private void UpdateAvailibleTargets()
	{
		availibleTargets = GetAvailibleTargets();
		System.Array.Sort(availibleTargets, DistanceComparator);
	}
	
	private void ClearTarget(int index)
	{
		targets[index] = null;
		lastAttackTimePerAttackType[index] = 0.0f;
	}
	
	private void UpdateTargets()
	{	
		UpdateAvailibleTargets();
		
		bool targetsAreSoFar = false;
		for(int i = 0; i < targets.Length; i++)
		{
			if(targetsAreSoFar)
			{
				ClearTarget(i);
				continue;
			}
			
			GameObject target = targets[i];
			BulletDefinition attackType = attackTypes[i];
			
			if(!IsTargetValid(target, attackType))
			{
				target = FindTargetForAttackType(attackType);
				if(target == null)
				{
					targetsAreSoFar = true;
					ClearTarget(i);
				}
				else
				{
					targets[i] = target;
				}
			}
		}
	}
	
	private void EnableRendering()
	{
		if(particleSystem != null)
		{
			particleSystem.Play();
		}
		
		if(renderer != null)
		{
			renderer.enabled = true;
		}
	}
	
	private void DisableRendering()
	{
		if(particleSystem != null)
		{
			particleSystem.Stop(true);
		}
		
		if(renderer != null)
		{
			renderer.enabled = false;
		}
	}
	
	private IEnumerator UpdateRenderingStateAction()
	{
		yield return new WaitForSeconds(delayBeforeRender);
		StopCoroutine("DisableRenderingAction");
		EnableRendering();
		StartCoroutine("DisableRenderingAction");
	}
	
	private void UpdateRenderingState()
	{
		if(renderWhileAttackingOnly && shouldUpdateRenderingState)
		{
			StopCoroutine("UpdateRenderingStateAction");
			StartCoroutine("UpdateRenderingStateAction");
		}
	}
	
	private IEnumerator DisableRenderingAction()
	{
		yield return new WaitForSeconds(renderingDuration);
		DisableRendering();
	}
	
	private void CreateAndThrowBullet(BulletDefinition bulletDefinition, GameObject target)
	{
		GameObject bulletPrefab = bulletDefinition.bullet;
		
		if(bulletPrefab == null)
		{
			throw new System.ArgumentNullException("Please specify bullet for your weapon: '" + name + "'");
		}
		
		GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position, bulletPrefab.transform.rotation);
		Bullet bulletComponent = bullet.GetComponent<Bullet>();
		if(bulletComponent == null)
		{
			Debug.LogError(bullet.name + " is invalid: bullet prefab must have Bullet component");
			return;
		}
		
		GameObject[] effects = RandomUtilites.Roll(bulletDefinition.additionalEffects, bulletDefinition.effectsChances);
		
		bulletComponent.Throw(target, bulletDefinition.damage, 
			bulletDefinition.attackType, effects);
	}
	
	private void AttackWithAttackType(int index)
	{	
		BulletDefinition attackType = attackTypes[index];
		float lastAttackTime = lastAttackTimePerAttackType[index];
		float currentTime = Time.time;
		
		if(currentTime - lastAttackTime < attackType.cooldown)
		{
			UpdateRenderingState();
			shouldUpdateRenderingState = false;
			return;
		}
		
		// avoid equals with 0.0f
		if(lastAttackTime < 0.1f)
		{
			lastAttackTimePerAttackType[index] = currentTime;
		}
		else
		{
			lastAttackTimePerAttackType[index] = lastAttackTime + attackType.cooldown;
		}
		
		GameObject target = targets[index];
		CreateAndThrowBullet(attackType, target);
		shouldUpdateRenderingState = true;
	}
	
	private void Attack()
	{
		bool isAttacking = false;
		
		for(int i = 0; i < targets.Length; i++)
		{
			if(targets[i] != null)
			{
				isAttacking = true;
				AttackWithAttackType(i);
			}
		}
	}
	
	private IEnumerator TargetsUpdatingAction()
	{
		while(true)
		{
			yield return new WaitForSeconds(UPDATE_TARGETS_DELAY);
			UpdateTargets();
		}
	}
	
	public static string GetAvailibleTargetTypesAsJoinedString(Weapon.BulletDefinition attackType, string separator = ", ")
	{
		string[] canAttack = new string[2];
		int count = 0;
		if(attackType.air)
		{
			canAttack[count] = Enum.GetName(typeof(Target.Type), Target.Type.AIR);
			count++;
		}
		if(attackType.ground)
		{
			canAttack[count] = Enum.GetName(typeof(Target.Type), Target.Type.GROUND);
			count++;
		}
		
		return string.Join(separator, canAttack, 0, count);
	}
	
	public BulletDefinition GetMainAttackType()
	{
		return attackTypes[0];
	}
	
	void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
		rendererComponent = renderer;
		if(renderWhileAttackingOnly)
		{
			DisableRendering();
		}
		
		Transform parent = transform.parent;
		if(parent != null)
		{
			tower = parent.gameObject.GetComponent<Tower>();
		}
		
		StartCoroutine(TargetsUpdatingAction());
	}
	
	void FixedUpdate() 
	{
		if(attackingEnabled)
		{
			Attack();
		}
	}
}
