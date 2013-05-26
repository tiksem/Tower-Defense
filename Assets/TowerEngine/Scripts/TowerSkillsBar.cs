using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BarWithCircleButtons))]
public class TowerSkillsBar : MonoBehaviour
{
	[System.Serializable]
	public class TowerUpgrade
	{
		public Tower tower;
		public Texture icon;
		public int goldCost;
	}
	
	public TowerUpgrade[] upgrades;
	
	private BarWithCircleButtons bar;
	
	void Start()
	{
		bar = GetComponent<BarWithCircleButtons>();
	}
	
	public bool GetTowerUpgradesCheckClicks(out TowerUpgrade towerUpgrade)
	{
		towerUpgrade = null;
		int buttonIndex = bar.GetClickedButtonIndex();
		if(buttonIndex < 0)
		{
			return false;
		}
		
		if(buttonIndex < upgrades.Length)
		{
			towerUpgrade = upgrades[buttonIndex];
		}
		
		return true;
	}
}
