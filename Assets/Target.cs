using UnityEngine;
using System.Collections.Generic;
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
	
	private int currentHP;
	private float currentPhysicalArmorCoefficient;
	private int currentPhysicalArmor;
	private HashSet<Weapon.AttackType> immunitiesSet = new HashSet<Weapon.AttackType>();
	private CharacterMotor characterMotorComponent;
	private List<GameObject> effects = new List<GameObject>();
	
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
	
	protected void Die()
	{
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
		if(characterMotorComponent == null)
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
		
		characterMotorComponent.movement.maxForwardSpeed *= coefficient;
		characterMotorComponent.movement.maxBackwardsSpeed *= coefficient;
		characterMotorComponent.movement.maxFallSpeed *= coefficient;
	}
	
	public void NotifyEffectDestroyed(GameObject effect)
	{
		effects.Remove(effect);
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
			return effects.Find((GameObject effect) 
				=> effect != null && effect.name == effectName
			) == null;
		}
		else
		{
			return true;
		}
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
		characterMotorComponent = GetComponent<CharacterMotor>();
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
