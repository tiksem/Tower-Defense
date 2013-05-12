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
	public GameObject[] availibleTowerPlaces;
	public float towerSize = 3;
	public Terrain terrain;
	public GameObject towerBuildButton;
	public GameObject towersBar;
	
	private MapState mapState = MapState.ACTIVE;
	public GameObject selectedTower;
	private int currentGold;
	private GuiEventsHandler towerBuildButtonHandler;
	private IDictionary<Vector2, string> towerNameByPlaceMap = new Dictionary<Vector2, string>();
	private MouseClickHandler mouseClickHandler = new MouseClickHandler();
	private Vector3 towerPosition;
	private TowersBar towersBarComponent;
	private TextureTrigger towerBuildButtonTrigger;
	
	public static TowerManager Instance
	{
		get
		{
			return instance;
		}
	}
	
	private void InitTowersBar()
	{
		if(towersBar == null)
		{
			return;
		}
		
		towersBarComponent = towersBar.GetComponent<TowersBar>();
		if(towersBarComponent == null)
		{
			throw new System.ArgumentException("towersBar must have TowersBar component");
		}
	}
	
	void OnValidate()
	{
		InitTowersBar();
	}
	
	void Awake()
	{
		if(instance != null)
		{
			throw new System.ApplicationException("TowerManager can not be created twice");
		}
		
		instance = this;
	}
	
	private Vector2 GetGridOffset(Vector3 position)
	{
		float offsetX = Utilities.GetModuloPart(position.x, towerSize);
		float offsetY = Utilities.GetModuloPart(position.y, towerSize);
		return new Vector2(offsetX, offsetY);
	}
	
	private Vector3 GetTowerPosition(RaycastHit raycastHit)
	{
		Vector3 requestedPosition = raycastHit.point;
		Vector3 hitObjectPosition = raycastHit.collider.bounds.min;
		
		Vector3 localPosition = requestedPosition - hitObjectPosition;
		localPosition.y = requestedPosition.y;
		localPosition.x = Utilities.RemoveModuloPart(localPosition.x, towerSize);
		localPosition.z = Utilities.RemoveModuloPart(localPosition.z, towerSize);
		localPosition.x += towerSize / 2;
		localPosition.z += towerSize / 2;
		hitObjectPosition.y = 0;
		requestedPosition = hitObjectPosition + localPosition;
		
		return requestedPosition;
	}
	
	public void NotifyTargetDestroyed(Target target)
	{
		
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
	
	private void SetTowersBarVisibility(bool value)
	{
		towersBar.SetActive(value);
	}
	
	private void ShowTowersBar()
	{
		SetTowersBarVisibility(true);
	}
	
	private void HideTowersBar()
	{
		SetTowersBarVisibility(false);
	}
	
	private void OpenTowerPlaceSelectionMenu()
	{
		mapState = MapState.SELECTING_TOWER_PLACE;
		ShowGrid();
	}
	
	private void BuildTower(Vector3 towerPosition, GameObject towerPrefab)
	{
		GameObject tower = PositionUtilities.InstantiateGameObjectAndPutCenterOnXZPlane(towerPrefab, towerPosition);
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
		towerBuildButtonTrigger.ChangeTexture(value);
	}
	
	private void ShowTowerBuildButton()
	{
		SetTowerBuildButtonVisibility(true);
	}
	
	private void HideTowerBuildButton()
	{
		SetTowerBuildButtonVisibility(false);
	}
	
	private void ReplaceTower(Vector3 towerPosition)
	{
		
	}
	
	private string GetTowerName(GameObject tower)
	{
		Tower towerComponent = tower.GetComponent<Tower>();
		return towerComponent.name;
	}
	
	private void HandleTowerPlaceSelection()
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
	
	private void OnClick()
	{
		switch(mapState)
		{
		case MapState.SELECTING_TOWER_PLACE:
			CheckTowerPlaceSelection();
			break;
		default:
			break;
		}
	}
	
	private bool CheckTowerPlaceSelection()
	{
		RaycastHit mouseOnGrid = MouseUtilities.FindFirstGameObjectHitInMouseRay(availibleTowerPlaces);
		if(mouseOnGrid.point == Vector3.zero)
		{
			return false;
		}
		
		towerPosition = GetTowerPosition(mouseOnGrid);
		HandleTowerPlaceSelection();
		return true;
	}
	
	private void CloseTowerBuildMenu()
	{
		mapState = MapState.ACTIVE;
		HideGrid();
		HideTowersBar();
		ShowTowerBuildButton(); 
	}
	
	private void OnTowerBuildButtonClick()
	{
		if(mapState == MapState.ACTIVE)
		{
			OpenTowerSelectionMenu();
		}
		else
		{
			CloseTowerBuildMenu();
		}
	}
	
	private void OnMapActiveClick()
	{
		
	}
	
	private void OpenTowerSelectionMenu()
	{
		mapState = MapState.SELECTING_TOWER;
		HideTowerBuildButton();
		ShowTowersBar();
	}
	
	private void UpdateSelectedTower()
	{
		selectedTower = towersBarComponent.GetSelectedTower();
		if(selectedTower != null)
		{
			OpenTowerPlaceSelectionMenu();
		}
	}
	
	private void UpdateClicks()
	{
		if(towerBuildButtonHandler.Update() != GuiEventsHandler.State.NONE)
		{
			return;
		}
		
		if(mapState == MapState.SELECTING_TOWER)
		{
			if(towersBarComponent.UpdateEvents())
			{
				UpdateSelectedTower();
				return;
			}
		}
		
		mouseClickHandler.Update();
	}
	
	// Use this for initialization
	void Start() 
	{
		InitTowersBar();
		HideGrid();
		HideTowersBar();
		towerBuildButtonHandler = new GuiEventsHandler(towerBuildButton);
		towerBuildButtonHandler.onMouseClick = OnTowerBuildButtonClick;
		//mouseClickHandler.isClickAccepted = IsMouseClickAccepted;
		mouseClickHandler.onClick = OnClick;
		towerBuildButtonTrigger = towerBuildButton.GetComponent<TextureTrigger>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateClicks();
	}
}
