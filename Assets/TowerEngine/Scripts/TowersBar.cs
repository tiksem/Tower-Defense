using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

[RequireComponent (typeof(GUITexture))]
public class TowersBar : MonoBehaviour
{
	public GameObject[] towers = new GameObject[5];
	public BarWithCircleButtons barSettings;
	
	private GuiEventsHandler guiEventsHandler;
	private GameObject selectedTower;
	private GUITexture barTexture;
	
	private void InitTexture()
	{
		if(barTexture == null)
		{
			barTexture = GetComponent<GUITexture>();
		}
		
		ResizeTexture();
	}
	
	private void UpdateBarSettings()
	{
		barSettings.Update(towers.Length + 1, barTexture);
	}
	
	void OnValidate()
	{
		InitTexture();
		UpdateBarSettings();
	}
	
	private void ResizeTexture()
	{
		GUIUtilities.ResizeGUITextureToFitScreenWidth(barTexture);
	}
	
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
			int clickedButtonIndex = barSettings.GetClickedButtonIndex();
			if(clickedButtonIndex < 0)
			{
				selectedTower = null;
				return true;
			}
			
			if(clickedButtonIndex < towers.Length)
			{
				OnTowerClick(clickedButtonIndex);
			}
			else
			{
				OnFractionsClick();
			}
			
			return true;
		}
		else
		{
			selectedTower = null;
			return false;
		}
	}
	
	void Start()
	{
		InitTexture();
		UpdateBarSettings();
		guiEventsHandler = new GuiEventsHandler(gameObject);
	}

	void Update()
	{
		
	}
}
