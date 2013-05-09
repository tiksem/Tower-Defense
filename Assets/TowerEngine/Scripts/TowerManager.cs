using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class TowerManager : MonoBehaviour 
{
	private static TowerManager instance;
		
	public enum MapState
	{
		ACTIVE,
		SELECTING_TOWER,
		SELECTING_TOWER_PLACE,
	}
	
	public int startGold = 200;
	public GameObject[] towers;
	public float towerSize = 3;
	public GameObject[] availibleTowerPlaces;
	public Terrain terrain;
	public GameObject towerBuildButton;
	
	private MapState mapState = MapState.ACTIVE;
	public GameObject selectedTower;
	private int currentGold;
	private GuiEventsHandler towerBuildButtonHandler;
	private IDictionary<Vector2, string> towerNameByPlaceMap = new Dictionary<Vector2, string>();
	
	public static TowerManager Instance
	{
		get
		{
			return instance;
		}
	}
	
	void OnValidate()
	{
		if(terrain == null)
		{
			return;
		}
	}
	
	void Awake()
	{
		if(instance != null)
		{
			throw new System.ApplicationException("TowerManager can not be created twice");
		}
		
		instance = this;
	}
	
	private Vector3 GetTowerPosition(Vector3 requestedPosition)
	{
		requestedPosition.x = Utilities.RemoveModuloPart(requestedPosition.x, towerSize);
		requestedPosition.z = Utilities.RemoveModuloPart(requestedPosition.z, towerSize);
		requestedPosition.y = terrain.SampleHeight(requestedPosition);
		return requestedPosition;
	}
	
	private void BuildTower()
	{
		
	}
	
	public void NotifyTargetDestroyed(Target target)
	{
		
	}
	
	public void OpenTowerBuildMenu()
	{
		OpenTowerSelectionMenu();
	}
	
	private void SetGridVisibility(bool value)
	{
		foreach(GameObject towerPlace in availibleTowerPlaces)
		{
			towerPlace.SetActive(value);
		}
	}
	
	private void ShowGrid()
	{
		SetGridVisibility(true);
	}
	
	private void HideGrid()
	{
		SetGridVisibility(false);
	}
	
	private void OpenTowerPlaceSelectionMenu()
	{
		mapState = MapState.SELECTING_TOWER_PLACE;
		ShowGrid();
	}
	
	private void BuildTower(Vector3 towerPosition, GameObject towerPrefab)
	{
		GameObject tower = (GameObject)Instantiate(towerPrefab, towerPosition, towerPrefab.transform.rotation);
		Tower towerComponent = tower.GetComponent<Tower>();
		currentGold -= towerComponent.goldPrice;
		towerNameByPlaceMap[new Vector2(towerPosition.x, towerPosition.z)] = tower.name;
	}
	
	private void BuildSelectedTower(Vector3 towerPosition)
	{
		BuildTower(towerPosition, selectedTower);
	}
	
	private void SetTowerBuildButtonVisibility(bool value)
	{
		if(towerBuildButton == null)
		{
			return;
		}
		
		towerBuildButton.SetActive(value);
	}
	
	private void ShowTowerBuildButton()
	{
		SetTowerBuildButtonVisibility(true);
	}
	
	private void HideTowerBuildButton()
	{
		SetTowerBuildButtonVisibility(false);
	}
	
	private void OnTowerSelectionClick()
	{
		
	}
	
	private void ReplaceTower(Vector3 towerPosition)
	{
		
	}
	
	private string GetTowerName(GameObject tower)
	{
		Tower towerComponent = tower.GetComponent<Tower>();
		return towerComponent.name;
	}
	
	private void HandleTowerPlaceSelection(Vector3 towerPosition)
	{
		string towerName = "";
		
		if(!towerNameByPlaceMap.TryGetValue(new Vector3(towerPosition.x, towerPosition.z), out towerName))
		{
			BuildSelectedTower(towerPosition);
		}
		else
		{
			string selectedTowerName = GetTowerName(selectedTower);
			if(towerName != selectedTowerName)
			{
				ReplaceTower(towerPosition);
			}
		}
	}
	
	private void OnTowerPlaceSelectionClick()
	{
		Vector3 mouseOnTerrain = MouseUtilities.GetMousePositionOnGameObject(terrain.gameObject, true);
		if(mouseOnTerrain == Vector3.zero)
		{
			return;
		}
		
		Vector3 towerPosition = GetTowerPosition(mouseOnTerrain);
		HandleTowerPlaceSelection(towerPosition);
	}
	
	private void OnTowerBuildButtonClick()
	{
		OpenTowerBuildMenu();
	}
	
	private void OnMapActiveClick()
	{
		
	}
	
	private void OpenTowerSelectionMenu()
	{
		mapState = MapState.SELECTING_TOWER;
		HideTowerBuildButton();
		OpenTowerPlaceSelectionMenu();
	}
	
	private void UpdateClicks()
	{
		if(towerBuildButtonHandler.Update())
		{
			return;
		}
		
		if(Input.GetMouseButton(0))
		{
			switch(mapState)
			{
			case MapState.SELECTING_TOWER_PLACE:
				OnTowerPlaceSelectionClick();
				break;
			case MapState.SELECTING_TOWER:
				OnTowerSelectionClick();
				break;
			case MapState.ACTIVE:
				OnMapActiveClick();
				break;
			}
		}
	}
	
	// Use this for initialization
	void Start() 
	{
		OnValidate();
		HideGrid();
		towerBuildButtonHandler = new GuiEventsHandler(towerBuildButton);
		towerBuildButtonHandler.onMouseClick = OnTowerBuildButtonClick;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateClicks();
	}
}
