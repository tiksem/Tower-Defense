using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

[RequireComponent (typeof(BarWithCircleButtons))]
public class TowersBar : MonoBehaviour
{
	public GameObject[] towers = new GameObject[5];
	
	private GuiEventsHandler guiEventsHandler;
	private GameObject selectedTower;
	private int selectedTowerIndex = -1;
	private BarWithCircleButtons bar;
	private int[] towersGold;

	public GameObject GetSelectedTower()
	{
		return selectedTower;
	}
	
	public int GetSelectedTowerIndex()
	{
		return selectedTowerIndex;
	}
	
	private void OnTowerClick(int towerIndex)
	{
		if(towerIndex >= 0 && towerIndex < towers.Length)
		{
			selectedTower = towers[towerIndex];	
			
			if(selectedTowerIndex >= 0)
			{
				BarWithCircleButtons.ButtonState selectedTowerState = bar.GetButtonState(selectedTowerIndex);
				if(selectedTowerState == BarWithCircleButtons.ButtonState.SELECTED)
				{
					bar.SetButtonState(selectedTowerIndex, BarWithCircleButtons.ButtonState.NORMAL);
				}
			}
			
			BarWithCircleButtons.ButtonState towerState = bar.GetButtonState(towerIndex);
			if(towerState != BarWithCircleButtons.ButtonState.DISABLED)
			{
				bar.SetButtonState(towerIndex, BarWithCircleButtons.ButtonState.SELECTED);
			}
		}
		
		selectedTowerIndex = towerIndex;
	}
	
	private void OnFractionsClick()
	{
		selectedTower = null;
	}
	
	public bool UpdateEvents()
	{
		if(guiEventsHandler.Update() == GuiEventsHandler.State.CLICK)
		{
			return true;
		}
		
		int clickedButtonIndex = bar.GetClickedButtonIndex();
		
		if(clickedButtonIndex == towers.Length)
		{
			OnTowerClick(-1);
			OnFractionsClick();
		}
		else if(clickedButtonIndex >= 0)
		{
			OnTowerClick(clickedButtonIndex);
			return true;
		}
			
		return false;
	}
	
	public void UpdateTowersGoldState(int currentGold)
	{
		for(int i = 0; i < towersGold.Length; i++)
		{
			if(currentGold < towersGold[i])
			{
				bar.SetButtonState(i, BarWithCircleButtons.ButtonState.DISABLED);
			}
		}
	}
	
	public int GetTowerGoldPrice(int towerIndex)
	{
		return towersGold[towerIndex];
	}
	
	public int TryBuyTower(int towerIndex, int currentGold)
	{
		int towerGold = towersGold[towerIndex];
		if(towerGold <= currentGold)
		{
			UpdateTowersGoldState(currentGold - towerGold);
			return towerGold;
		}
		else
		{
			return -1;
		}
	}
	
	public int TryBuySelectedTower(int currentGold)
	{
		return TryBuyTower(selectedTowerIndex, currentGold);
	}
	
	private void InitTowersGold()
	{
		int length = towers.Length;
		towersGold = new int[length];
		for(int i = 0; i < length; i++)
		{
			GameObject towerObject = towers[i];
			if(towerObject == null)
			{
				towersGold[i] = -1;
				continue;
			}
			
			Tower tower = towerObject.GetComponent<Tower>();
			towersGold[i] = tower.goldPrice;
		}
	}
	
	void Start()
	{
		bar = gameObject.GetComponent<BarWithCircleButtons>();
		guiEventsHandler = new GuiEventsHandler(gameObject);
		InitTowersGold();
	}

	void Update()
	{
		
	}
}
