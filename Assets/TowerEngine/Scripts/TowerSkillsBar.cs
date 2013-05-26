using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BarWithCircleButtons))]
public class TowerSkillsBar : MonoBehaviour
{
	[System.Serializable]
	public struct TowerUpgrade
	{
		public Tower tower;
		public Texture icon;
		public int goldCost;
	}
	
	/*public bool GetSelectedTowerUpgrade(TowerUpgrade )
	{
		
	}*/
	
	public TowerUpgrade[] upgrades;
	
	private BarWithCircleButtons bar;
	
	private void DoUpgradeIfCan(TowerUpgrade towerUpgrade)
	{
		//if(towerUpgrade.goldCost < TowerManager.
	}
	
	private void HandleClicks()
	{
		int upgradeIndex = bar.GetClickedButtonIndex();
		if(upgradeIndex < 0 || upgradeIndex >= upgrades.Length)
		{
			return;
		}
		
		TowerUpgrade towerUpgrade = upgrades[upgradeIndex];
	}
	
	void Start() 
	{
		bar = GetComponent<BarWithCircleButtons>();
	}
}
