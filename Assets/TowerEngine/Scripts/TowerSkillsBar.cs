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
	
	void Awake()
	{
		bar = GetComponent<BarWithCircleButtons>();
	}
	
	public void SetUpgrades(TowerUpgrade[] upgrades)
	{
		this.upgrades = upgrades;
		
		int length = Mathf.Min(upgrades.Length, bar.buttonsCount - 1);
		for(int i = 0; i < length; i++)
		{
			bar.buttonTextures[i].normalState = upgrades[i].icon;
		}
	}
	
	public int GetClickedButtonIndex()
	{
		return bar.GetClickedButtonIndex();
	}
	
	public int GetButtonsCount()
	{
		return bar.GetButtonsCount();
	}
	
	public TowerUpgrade GetTowerUpgradeByIndex(int index)
	{
		if(index < 0)
		{
			return null;
		}
		
		if(index < upgrades.Length)
		{
			return upgrades[index];
		}
		
		return null;
	}
}
