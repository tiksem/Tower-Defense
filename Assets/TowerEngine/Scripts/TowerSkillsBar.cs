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
