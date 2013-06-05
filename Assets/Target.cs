using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using AssemblyCSharp;

public class Target : MonoBehaviour 
{
	public enum ArmorType
	{
		LIGHT,
		NORMAL,
		HEAVY,
		SHELL
		
	}
	
	private static readonly Weapon.AttackType[] magicAttackTypes = new Weapon.AttackType[]{
		Weapon.AttackType.DEATH,
		Weapon.AttackType.FIRE,
		Weapon.AttackType.GROUND,
		Weapon.AttackType.HOLY,
		Weapon.AttackType.ICE,
		Weapon.AttackType.POISON,
		Weapon.AttackType.WIND
	};
	
	private static void SetMagicResistanceForArmorType(IDictionary<Weapon.AttackType, float> armorType, 
		float value)
	{
		foreach(Weapon.AttackType attackType in magicAttackTypes)
		{
			armorType[attackType] = value;
		}
	}
	
	private static IDictionary<ArmorType, IDictionary<Weapon.AttackType, float> > ARMOR_TABLE;
	static Target()
	{
		ARMOR_TABLE = new Dictionary<ArmorType, IDictionary<Weapon.AttackType, float> >();
		
		var normalArmor = new Dictionary<Weapon.AttackType, float>();
		ARMOR_TABLE[ArmorType.NORMAL] = normalArmor;
		
		SetMagicResistanceForArmorType(normalArmor, 1.0f);
		normalArmor[Weapon.AttackType.PIERCING] = 1.0f;
		normalArmor[Weapon.AttackType.SIEGE] = 1.0f;
		normalArmor[Weapon.AttackType.NORMAL] = 1.0f;
		
		var lightArmor = new Dictionary<Weapon.AttackType, float>();
		ARMOR_TABLE[ArmorType.LIGHT] = lightArmor;
		
		SetMagicResistanceForArmorType(lightArmor, 0.65f);
		lightArmor[Weapon.AttackType.PIERCING] = 1.5f;
		lightArmor[Weapon.AttackType.SIEGE] = 1.0f;
		lightArmor[Weapon.AttackType.NORMAL] = 1.5f;
		
		var heavyArmor = new Dictionary<Weapon.AttackType, float>();
		ARMOR_TABLE[ArmorType.HEAVY] = heavyArmor;
		
		SetMagicResistanceForArmorType(heavyArmor, 1.5f);
		heavyArmor[Weapon.AttackType.PIERCING] = 0.65f;
		heavyArmor[Weapon.AttackType.SIEGE] = 1.0f;
		heavyArmor[Weapon.AttackType.NORMAL] = 0.65f;
		
		var shellArmor = new Dictionary<Weapon.AttackType, float>();
		ARMOR_TABLE[ArmorType.SHELL] = shellArmor;
		
		SetMagicResistanceForArmorType(shellArmor, 1.0f);
		shellArmor[Weapon.AttackType.PIERCING] = 0.65f;
		shellArmor[Weapon.AttackType.SIEGE] = 1.5f;
		shellArmor[Weapon.AttackType.NORMAL] = 1.0f;
	}
	
	private static bool IsMagicAttackType(Weapon.AttackType attackType)
	{
		return Array.IndexOf(magicAttackTypes, attackType) >= 0;
	}
	
	private static readonly float PHYSICAL_ARMOR_PERCENTS_PER_POINT = 0.01f;
	
	[System.Serializable]
	public class Immunities
	{
		public bool death = false;
		public bool fire = false;
		public bool ground = false;
		public bool holy = false;
		public bool ice = false;
		public bool poison = false;
		public bool wind = false;
	}
	
	public int maxHP = 100;
	public ArmorType armorType = ArmorType.NORMAL;
	public Immunities immunities = new Immunities();
	public int physicalArmor = 0;
	public int goldForKill = 1;
	public GameObject goldForKillPointerPrefab;
	public AudioClip deathSound;
	public GameObject deathEffect;
	public float deathEffectDuration = 1.0f;
	
	private int currentHP;
	private float currentPhysicalArmorCoefficient;
	private int currentPhysicalArmor;
	private HashSet<Weapon.AttackType> immunitiesSet = new HashSet<Weapon.AttackType>();
	private NavMeshAgent navMeshAgent;
	private List<GameObject> effects = new List<GameObject>();
	
	public NavMeshAgent GetNavMeshAgent()
	{
		return navMeshAgent;
	}
	
	public float GetSpeed()
	{
		return navMeshAgent.speed;
	}
	
	public float GetAngularSpeed()
	{
		return navMeshAgent.angularSpeed;
	}
	
	public void SetSpeeds(float speed, float angularSpeed)
	{
		navMeshAgent.speed = speed;
		navMeshAgent.angularSpeed = angularSpeed;
	}
	
	private void ValidateImmunities()
	{
		immunitiesSet.Clear();
		
		if(immunities.death)
		{
			immunitiesSet.Add(Weapon.AttackType.DEATH);
		}
		
		if(immunities.fire)
		{
			immunitiesSet.Add(Weapon.AttackType.FIRE);
		}
		
		if(immunities.ice)
		{
			immunitiesSet.Add(Weapon.AttackType.ICE);
		}
		
		if(immunities.ground)
		{
			immunitiesSet.Add(Weapon.AttackType.GROUND);
		}
		
		if(immunities.holy)
		{
			immunitiesSet.Add(Weapon.AttackType.HOLY);
		}
		
		if(immunities.poison)
		{
			immunitiesSet.Add(Weapon.AttackType.POISON);
		}
		
		if(immunities.wind)
		{
			immunitiesSet.Add(Weapon.AttackType.WIND);
		}
	}
	
	void Awake()
	{
		OnValidate();
	}
	
	void OnValidate()
	{
		ValidateImmunities();
		CurrentPhysicalArmor = physicalArmor;
	}
	
	public int CurrentPhysicalArmor
	{
		get
		{
			return currentPhysicalArmor;
		}
		set
		{
			currentPhysicalArmor = value;
			
			if(currentPhysicalArmor >= 0)
			{
				float initialCoefficient = 0.0f;
				
				for(var i = 0; i < currentPhysicalArmor; i++)
				{
					initialCoefficient += (PHYSICAL_ARMOR_PERCENTS_PER_POINT * (1.0f - initialCoefficient));
				}
				
				currentPhysicalArmorCoefficient = 1.0f - initialCoefficient;
			}
			else
			{
				currentPhysicalArmorCoefficient += -currentPhysicalArmor * PHYSICAL_ARMOR_PERCENTS_PER_POINT;
			}
		}
	}
	
	public bool HasImmunity(Weapon.AttackType attackType)
	{
		return immunitiesSet.Contains(attackType);
	}
	
	private float GetDamageCoefficientForAttackType(Weapon.AttackType attackType)
	{
		return ARMOR_TABLE[armorType][attackType];
	}
	
	private void ShowReceivedGold()
	{
		if(goldForKillPointerPrefab == null)
		{
			return;
		}
		
		Vector3 center = Rendering.GetObjectCenter(gameObject);
		center = GUIUtilities.WorldToGUIElementPositionPoint(center);
		
		GUIText goldText = Utilities.InstantiateAndGetComponent<GUIText>(goldForKillPointerPrefab, center);
		if(goldText == null)
		{
			throw new System.ArgumentException("goldForKillPointerPrefab must have GUIText component");
		}
		
		goldText.text += goldForKill;
	}
	
	private void PlaySound(AudioClip audio)
	{
		if(audio == null)
		{
			return;
		}
		
		AudioSource.PlayClipAtPoint(audio, transform.position);
	}
	
	private void PlayDeathSound()
	{
		PlaySound(deathSound);
	}
	
	private void ShowDeathEffect()
	{
		if(deathEffect != null)
		{
			Vector3 center = Rendering.GetObjectCenter(gameObject);
			GameObject effect = (GameObject)Instantiate(deathEffect, center, deathEffect.transform.rotation);
			FloatTimer.AttachTo(effect, deathEffectDuration);
		}
	}
	
	protected void Die()
	{
		TowerManager.Instance.NotifyTargetDestroyed(this);
		ShowReceivedGold();
		PlayDeathSound();
		ShowDeathEffect();
		Destroy(gameObject);
	}
	
	public void Damage(Weapon.AttackType attackType, int value)
	{
		float floatValue = value;
		
		if(HasImmunity(attackType))
		{
			return;
		}
		
		floatValue *= GetDamageCoefficientForAttackType(attackType);
		
		if(!IsMagicAttackType(attackType))
		{
			floatValue *= currentPhysicalArmorCoefficient;
		}
		
		currentHP -= (int)Mathf.Round(floatValue);
	}
	
	public void ChangeMovementSpeed(float coefficient, Weapon.AttackType? attackType = null)
	{
		if(navMeshAgent == null)
		{
			return;
		}
		
		if(coefficient < 0.0f)
		{
			throw new System.ArgumentException("ChangeMovementSpeed coefficient should be positive");
		}
		
		if(attackType != null && HasImmunity((Weapon.AttackType)attackType))
		{
			return;
		}
		
		navMeshAgent.speed *= coefficient;
		navMeshAgent.angularSpeed *= coefficient;
	}
	
	public void NotifyEffectDestroyed(GameObject effect)
	{
		effects.Remove(effect);
	}
	
	public bool IsEffectAttachedButThis(TargetHitEffect targetHitEffect)
	{
		return effects.Find((GameObject effect) => 
		{
			return effect != targetHitEffect.gameObject && effect.name == targetHitEffect.name;
		}) != null;
	}
	
	public bool IsEffectAttached(TargetHitEffect targetHitEffect)
	{
		return effects.Find((GameObject effect) => effect.name == targetHitEffect.name) != null;
	}
	
	private bool CheckEffect(GameObject effectPrefab)
	{
		TargetHitEffect effectComponent = effectPrefab.GetComponent<TargetHitEffect>();
		
		if(!effectComponent.CanBeAttachedToTarget(this))
		{
			return false;
		}
		
		if(!effectComponent.CanStack())
		{
			string effectName = effectPrefab.name;
			GameObject find = effects.Find((GameObject effect) 
				=> effect != null && effect.name == effectName
			);
			
			if(find != null)
			{
				if(effectComponent.ShouldBeReplaced())
				{
					effects.Remove(find);
					Destroy(find);
				}
				else
				{
					return false;
				}
			}
			
		}
		
		return true;
	}
	
	public void AttachEffect(GameObject effectPrefab)
	{
		if(effectPrefab == null)
		{
			throw new ArgumentNullException();
		}
		
		if(!CheckEffect(effectPrefab))
		{
			return;
		}
		
		GameObject effect = Utilities.InstantiateChildOf(effectPrefab, gameObject);
		
		TargetHitEffect effectComponent = effect.GetComponent<TargetHitEffect>();
		if(effectComponent == null)
		{
			throw new System.ArgumentException("effect must have TargetHitEffect component");
		}
		effectComponent.target = gameObject;
		
		effects.Add(effect);
	}
	
	public void AttachEffects(GameObject[] effects)
	{
		if(effects == null)
		{
			throw new ArgumentNullException();
		}
		
		foreach(GameObject effect in effects)
		{
			AttachEffect(effect);
		}
	}
	
	public int GetCurrentHP()
	{
		return currentHP;
	}
	
	private void DestroyEffects()
	{
		foreach(GameObject effect in effects)
		{
			Destroy(effect);
		}
	}
	
	void OnDestroy()
	{
		DestroyEffects();
	}
	
	// Use this for initialization
	void Start() 
	{
		currentHP = maxHP;
		navMeshAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if(currentHP <= 0)
		{
			Die();
		}
	}
}
