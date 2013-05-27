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
		public Texture disabledIcon;
		public int goldCost;
	}
	
	private TowerUpgrade[] upgrades;
	
	private BarWithCircleButtons bar;
	
	void Awake()
	{
		bar = GetComponent<BarWithCircleButtons>();
	}
	
	public BarWithCircleButtons.ButtonState GetButtonState(int index)
	{
		return bar.GetButtonState(index);
	}
	
	public int GetActiveButtonsCount()
	{
		return Mathf.Min(upgrades.Length, bar.buttonsCount - 1);
	}
	
	public void SetUpgrades(TowerUpgrade[] upgrades)
	{
		this.upgrades = upgrades;
		
		int length = GetActiveButtonsCount();
		for(int i = 0; i < length; i++)
		{
			BarWithCircleButtons.Button button = bar.buttonTextures[i];
			TowerUpgrade upgrade = upgrades[i];
			button.normalState = upgrade.icon;
			button.disabledState = upgrade.disabledIcon;
			
			if(upgrade.tower != null)
			{
				bar.SetText(i, upgrade.goldCost);
			}
			else
			{
				bar.SetText(i, "");
			}
		}
		
		for(int i = length; i < bar.buttonsCount - 1; i++)
		{
			BarWithCircleButtons.Button button = bar.buttonTextures[i];
			button.normalState = null;
			button.selectedState = null;
			button.additionalText = "";
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
	
	public void UpdateButtonsGoldState(int currentGold)
	{
		if(upgrades == null)
		{
			return;
		}
		
		int length = GetActiveButtonsCount();
		for(int i = 0; i < length; i++)
		{
			TowerUpgrade upgrade = upgrades[i];
			if(upgrade.tower != null)
			{
				if(upgrade.goldCost > currentGold)
				{
					bar.SetButtonState(i, BarWithCircleButtons.ButtonState.DISABLED);
				}
				else
				{
					bar.SetButtonState(i, BarWithCircleButtons.ButtonState.NORMAL);
				}
			}
		}
	}
}
