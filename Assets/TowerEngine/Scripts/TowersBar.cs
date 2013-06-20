using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

[RequireComponent (typeof(BarWithCircleButtons))]
public class TowersBar : MonoBehaviour
{
	public GameObject[] towers = new GameObject[5];
	
	private GameObject selectedTower;
	private int selectedTowerIndex = -1;
	private BarWithCircleButtons bar;
	private int[] towersGold;
	
	private List<Tower> allTowers = new List<Tower>();

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
			else
			{
				Messenger.Instance.ShowMessage(Messenger.Instance.notEnoughGoldMessage);
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
		int clickedButtonIndex = bar.GetClickedButtonIndex();
		
		if(clickedButtonIndex == towers.Length)
		{
			OnTowerClick(-1);
			OnFractionsClick();
			return true;
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
			else if(bar.GetButtonState(i) == BarWithCircleButtons.ButtonState.DISABLED)
			{
				bar.SetButtonState(i, BarWithCircleButtons.ButtonState.NORMAL);
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
	
	public int GetTowerIdByPrefabObject(GameObject prefab)
	{
		Tower tower = prefab.GetComponent<Tower>();
		return allTowers.IndexOf(tower);
	}
	
	public void Deselect()
	{
		if(selectedTowerIndex < 0)
		{
			return;
		}
		
		if(bar.GetButtonState(selectedTowerIndex) == BarWithCircleButtons.ButtonState.SELECTED)
		{
			bar.SetButtonState(selectedTowerIndex, BarWithCircleButtons.ButtonState.NORMAL);
		}
		
		selectedTowerIndex = -1;
	}
	
	public Tower GetTowerById(int id)
	{
		return allTowers[id];
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
			
			int gold = tower.goldPrice;
			towersGold[i] = gold;
			bar.SetText(i, gold);
		}
	}
	
	private void AddTowerAndUpgradesToAllTowers(Tower tower)
	{
		if(tower == null)
		{
			return;	
		}
		
		allTowers.Add(tower);
		
		if(tower.upgrades == null)
		{
			return;
		}
		
		for(int i = 0; i < tower.upgrades.Length; i++)
		{
			Tower towerToAdd = tower.upgrades[i].tower;
			AddTowerAndUpgradesToAllTowers(towerToAdd);
		}
	}
	
	void InitAllTowers()
	{
		foreach(GameObject tower in towers)
		{
			AddTowerAndUpgradesToAllTowers(tower.GetComponent<Tower>());
		}
	}
	
	void Awake()
	{
		bar = gameObject.GetComponent<BarWithCircleButtons>();
		InitTowersGold();
		InitAllTowers();
	}

	void Update()
	{
		
	}
}
