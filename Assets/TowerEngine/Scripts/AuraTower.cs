using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class AuraTower : Tower
{
	public float damageBonus = 1.5f;
	public float radius = 5.0f;
	public bool affectOtherAuraTowers = false;
	public Projector auraEffectPrefab;
	public float effectProjectorHeight = 3.0f;
	
	private class WeaponAttackTypeInfo
	{
		public int damage;
	}
	
	private IDictionary<Weapon, WeaponAttackTypeInfo[]> lastWeaponsStats = new Dictionary<Weapon, WeaponAttackTypeInfo[]>();
	private List<Projector> effects = new List<Projector>();
	
	public IEnumerable GetTowers()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
		foreach(Collider collider in colliders)
		{
			GameObject gameObject =	collider.gameObject;
			Tower tower = gameObject.GetComponent<Tower>();
			if(tower != null)
			{
				if(affectOtherAuraTowers || !(tower is AuraTower))
				{
					yield return tower;
				}
			}
		}
	}
	
	public static IEnumerable GetWeaponsOfTowers(IEnumerable towers)
	{
		foreach(Tower tower in towers)
		{
			Transform transform = tower.transform;
			int childCount = transform.GetChildCount();
			for(int i = 0; i < childCount; i++)
			{
				Transform childTransform = transform.GetChild(i);
				GameObject child = childTransform.gameObject;
				Weapon weapon = child.GetComponent<Weapon>();
				if(weapon != null)
				{
					yield return weapon;
				}
			}
		}
	}
	
	public IEnumerable GetWeapons()
	{
		IEnumerable towers = GetTowers();
		return GetWeaponsOfTowers(towers);
	}
	
	private void ReturnStats()
	{
		foreach(KeyValuePair<Weapon, WeaponAttackTypeInfo[]> weaponStats in lastWeaponsStats)
		{
			Weapon weapon = weaponStats.Key;
			WeaponAttackTypeInfo[] stats = weaponStats.Value;
			
			Weapon.BulletDefinition[] attackTypes = weapon.attackTypes;
			for(int i = 0; i < attackTypes.Length; i++)
			{
				WeaponAttackTypeInfo lastAttackTypeStats = stats[i];
				int lastDamage = lastAttackTypeStats.damage;
				
				Weapon.BulletDefinition attackType = attackTypes[i];
				attackType.damage = lastDamage;
			}
		}
	}
	
	private void SaveStats(IEnumerable weapons)
	{
		lastWeaponsStats.Clear();	
		
		foreach(Weapon weapon in weapons)
		{
			WeaponAttackTypeInfo[] stats = new WeaponAttackTypeInfo[weapon.attackTypes.Length];
			for(int i = 0; i < stats.Length; i++)
			{
				WeaponAttackTypeInfo statsOfWeapon = stats[i] = new WeaponAttackTypeInfo();
				Weapon.BulletDefinition attackType = weapon.attackTypes[i];
				
				statsOfWeapon.damage = attackType.damage;
				
				lastWeaponsStats.Add(weapon, stats);
			}
		}
	}
	
	private void DestroyEffects()
	{
		foreach(Projector effect in effects)
		{
			if(effect != null)
			{
				Destroy(effect.gameObject);
			}
		}
		
		effects.Clear();
	}
	
	private void AttachProjectorEffectToTower(Tower tower)
	{
		Vector3 towerCenter = Rendering.GetObjectCenter(tower.gameObject);
		towerCenter.y += effectProjectorHeight;
		Projector effect = (Projector)Instantiate(auraEffectPrefab, towerCenter, auraEffectPrefab.transform.rotation);
		effects.Add(effect);
	}
	
	private void AttachProjectorEffectToTowers(IEnumerable towers, Tower destroyedTower = null)
	{
		foreach(Tower tower in towers)
		{
			if(tower == this)
			{
				continue;
			}
			
			if(tower != destroyedTower)
			{
				AttachProjectorEffectToTower(tower);
			}
		}
	}
	
	void OnDestroy()
	{
		ReturnStats();
		DestroyEffects();
	}
	
	private void ApplyAura(Tower destroyedTower = null)
	{
		IEnumerable towers = GetTowers();
		IEnumerable weapons = GetWeaponsOfTowers(towers);
		
		SaveStats(weapons);
		
		foreach(Weapon weapon in weapons)
		{
			for(int i = 0; i < weapon.attackTypes.Length; i++)
			{
				Weapon.BulletDefinition attackType = weapon.attackTypes[i];
				attackType.damage = Mathf.RoundToInt(attackType.damage * damageBonus);
			}
		}
		
		DestroyEffects();
		AttachProjectorEffectToTowers(towers, destroyedTower);
	}
	
	protected override void Start()
	{
		base.Start();
		UpdateTowers();
	}
	
	public override void NotifyNewTowerBuilt()
	{
		UpdateTowers();
	}
	
	public override void NotifySomeTowerDestroyed(Tower destroyedTower)
	{
		UpdateTowers(destroyedTower);
	}
	
	public void UpdateTowers(Tower destroyedTower = null)
	{
		ReturnStats();
		ApplyAura(destroyedTower);
	}
}
