using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Weapon : MonoBehaviour 
{
	[System.Serializable]
	public class BulletDefinition
	{
		public String name = "Untitled";
		
		public GameObject bullet;
	
		public float cooldown = 1.0f;
		public float range = 1000.0f;
	}
	
	public bool attackingEnabled = false;
	public BulletDefinition[] attackTypes;
	
	private GameObject[] targets;
	private GameObject[] availibleTargets;
	private float[] lastAttackTimePerAttackType;
	
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
	
	// Use this for initialization
	void Start () 
	{
		
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
	
	private GameObject FindTargetForAttackType(BulletDefinition attackType)
	{
		int index = Array.BinarySearch(availibleTargets, attackType.range, new AssemblyCSharp.ComparisonComparer<object>(DistanceComparator));
		if(index < 0)
		{
			index = -index - 2;
 			if(index < 0)
			{
				return null;
			}
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
	
	private void UpdateAvailibleTargets()
	{
		availibleTargets = GameObject.FindGameObjectsWithTag("Target");
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
	
	private void StartAttacking()
	{
		UpdateTargets();
		Attack();
	}
	
	private void CreateAndThrowBullet(GameObject bulletPrefab, GameObject target)
	{
		GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position, transform.rotation);
		Bullet bulletComponent = bullet.GetComponent<Bullet>();
		if(bulletComponent == null)
		{
			Debug.LogError(bullet.name + " is invalid: bullet prefab must have Bullet component");
			return;
		}
		bulletComponent.target = target;
	}
	
	private void AttackWithAttackType(int index)
	{	
		BulletDefinition attackType = attackTypes[index];
		float lastAttackTime = lastAttackTimePerAttackType[index];
		float currentTime = Time.time;
		
		if(currentTime - lastAttackTime < attackType.cooldown)
		{
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
		CreateAndThrowBullet(attackType.bullet, target);
	}
	
	private void Attack()
	{
		for(int i = 0; i < targets.Length; i++)
		{
			if(targets[i] != null)
			{
				AttackWithAttackType(i);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(attackingEnabled)
		{
			StartAttacking();
		}
	}
}
