using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class StatsDescriptionSettings : MonoBehaviour
{
	public static StatsDescriptionSettings instance;
	
	public string towerFooterTextFormat = "The tower can attack {0} units. {1} and {2}";
	public string efficientAgainstFooterTextFormat = "It is efficient against {0}";
	public string unefficientAgainstFooterTextFormat = "unefficient against {0}";
	public string auraTowerFooterTextFormat = "The tower gives bonuses to nearly towers.";
	
	public string[] holyAttackTypeEfficientAgainst;
	public string[] piercingAttackTypeEfficientAgainst;
	public string[] siegeAttackTypeEfficientAgainst;
	
	public string[] holyAttackTypeUnefficientAgainst;
	public string[] piercingAttackTypeUnefficientAgainst;
	public string[] siegeAttackTypeUnefficientAgainst;
	
	private string[] efficientAgainstMessageMap;
	private string[] unefficientAgainstMessageMap;
	
	private string[] GetEfficientAgainstMessageMapFor(Weapon.AttackType attackType)
	{
		switch(attackType)
		{
		case Weapon.AttackType.HOLY:
			return holyAttackTypeEfficientAgainst;
		case Weapon.AttackType.PIERCING:
			return piercingAttackTypeEfficientAgainst;
		case Weapon.AttackType.SIEGE:
			return siegeAttackTypeEfficientAgainst;
		}
		
		return null;
	}
	
	private string[] GetUnefficientAgainstMessageMapFor(Weapon.AttackType attackType)
	{
		switch(attackType)
		{
		case Weapon.AttackType.HOLY:
			return holyAttackTypeUnefficientAgainst;
		case Weapon.AttackType.PIERCING:
			return piercingAttackTypeUnefficientAgainst;
		case Weapon.AttackType.SIEGE:
			return siegeAttackTypeUnefficientAgainst;
		}
		
		return null;
	}
	
	private string[] InitAttackTypeMessageMap(Func<Weapon.AttackType, string[]> getter)
	{
		Array values = Enum.GetValues(typeof(Weapon.AttackType));
		string[] map = new string[values.Length];
			
		foreach(Weapon.AttackType attackType in values)
		{
			string[] messages = getter(attackType);
			string message = string.Join(", ", messages);
			map[(int)attackType] = message;
		}
		
		return map;
	}
	
	public string GetEfficientAgainstMessage(Weapon.AttackType attackType)
	{
		return string.Format(efficientAgainstFooterTextFormat, efficientAgainstMessageMap[(int)attackType]);
	}
	
	public string GetUnefficientAgainstMessage(Weapon.AttackType attackType)
	{
		return string.Format(unefficientAgainstFooterTextFormat, unefficientAgainstMessageMap[(int)attackType]);
	}
	
	private void InitMessageMaps()
	{
		efficientAgainstMessageMap = InitAttackTypeMessageMap(GetEfficientAgainstMessageMapFor);
		unefficientAgainstMessageMap = InitAttackTypeMessageMap(GetUnefficientAgainstMessageMapFor);
	}
	
	void Awake()
	{
		if(Utilities.InitSingleton(ref instance, this))
		{
			InitMessageMaps();
		}
	}
}
