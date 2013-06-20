using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class TowerDescriptionGUI : StatsDescriptionGUI
	{
		private Tower tower;
		private Weapon weapon;
		private Weapon.BulletDefinition attackType;
		
		private string GetFooterMessage()
		{
			string canAttack = Weapon.GetAvailibleTargetTypesAsJoinedString(attackType);
			string unefficient = StatsDescriptionSettings.instance.GetUnefficientAgainstMessage(attackType.attackType);
			string efficient = StatsDescriptionSettings.instance.GetEfficientAgainstMessage(attackType.attackType);
			
			return string.Format(StatsDescriptionSettings.instance.towerFooterTextFormat, canAttack, efficient, unefficient);
		}
		
		public void SetTower(Tower tower)
		{
			this.tower = tower;
			headText = tower.name;
			weapon = tower.GetMainWeapon();
			attackType = weapon.GetMainAttackType();
			footerText = GetFooterMessage();
			
			Validate();
		}

	}
}

