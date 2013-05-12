using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

[RequireComponent (typeof(BarWithCircleButtons))]
public class TowersBar : MonoBehaviour
{
	public GameObject[] towers = new GameObject[5];
	
	private GuiEventsHandler guiEventsHandler;
	private GameObject selectedTower;
	private BarWithCircleButtons bar;

	public GameObject GetSelectedTower()
	{
		return selectedTower;
	}
	
	private void OnTowerClick(int towerIndex)
	{
		selectedTower = towers[towerIndex];
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
		if(clickedButtonIndex < 0)
		{
			selectedTower = null;
			return false;
		}
			
		if(clickedButtonIndex < towers.Length)
		{
			OnTowerClick(clickedButtonIndex);
			return true;
		}
		else if(clickedButtonIndex ==  towers.Length - 1)
		{
			OnFractionsClick();
			return true;
		}
		else
		{
			 selectedTower = null;
		}
			
		return false;
	}
	
	void Start()
	{
		bar = gameObject.GetComponent<BarWithCircleButtons>();
		guiEventsHandler = new GuiEventsHandler(gameObject);
	}

	void Update()
	{
		
	}
}
