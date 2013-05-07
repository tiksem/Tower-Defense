using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class TowerManager : MonoBehaviour 
{
	private static TowerManager instance;
		
	public enum MapState
	{
		ACTIVE,
		BUILDING_TOWER,
	}
	
	public int startGold = 200;
	public GameObject[] towers;
	public float towerSize = 3;
	public GameObject[] availibleTowerPlaces;
	public Terrain terrain;
	
	private TowerPlace[] towerPlaces;
	private MouseClickHandler mouseClickHandler = new MouseClickHandler();
	private MapState mapState = MapState.ACTIVE;
	private GameObject selectedTower;
	private int currentGold;
	
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
		
		towerPlaces = new TowerPlace[availibleTowerPlaces.Length];
		for(int i = 0; i < towerPlaces.Length; i++)
		{
			GameObject gameObject = availibleTowerPlaces[i];
			if(gameObject == null)
			{
				continue;
			}
			
			TowerPlace towerPlace = new TowerPlace(gameObject.renderer.bounds, towerSize);
			towerPlaces[i] = towerPlace;
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
	
	private Vector3 GetTowerPosition(TowerPlace towerPlace, Vector3 requestedPosition)
	{
		if(towerPlace == null)
		{
			return Vector3.zero;
		}
		
		Vector2 requestedPosition2D = new Vector2(requestedPosition.x, requestedPosition.z);
		Vector2 result = towerPlace.CalculatePosition(requestedPosition2D);
		float x = result.x;
		float y = result.y;
		float z = terrain.SampleHeight(new Vector3(x, requestedPosition.y, y));
		return new Vector3(x, y, z);
		
	}
	
	private Vector3 GetTowerPosition(Vector3 requestedPosition)
	{
		TowerPlace towerPlace = GetTowerPlace(requestedPosition);
		return GetTowerPosition(towerPlace, requestedPosition);
	}
	
	private TowerPlace GetTowerPlace(Vector3 requestedPosition)
	{
		Vector2 requestedPosition2D = new Vector2(requestedPosition.x, requestedPosition.z);
		TowerPlace towerPlace = System.Array.Find(towerPlaces, (TowerPlace obj) => obj.IsIn(requestedPosition2D));
		return towerPlace;
	}
	
	private void BuildTower()
	{
		
	}
	
	public void NotifyTargetDestroyed(Target target)
	{
		
	}
	
	public void OpenTowerBuildMenu()
	{
		StartTowerBuildingAction();
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
	
	private void StartTowerBuildingAction()
	{
		mapState = MapState.BUILDING_TOWER;
		ShowGrid();
	}
	
	private void BuildTower(Vector3 towerPosition, GameObject towerPrefab)
	{
		GameObject tower = (GameObject)Instantiate(towerPrefab, towerPosition, towerPrefab.transform.rotation);
		Tower towerComponent = tower.GetComponent<Tower>();
		currentGold -= towerComponent.goldPrice;
	}
	
	private void BuildSelectedTower(Vector3 towerPosition)
	{
		BuildTower(towerPosition, selectedTower);
	}
	
	private void OnTowerBuildingActionClick()
	{
		Vector3 mouseOnTerrain = MouseUtilities.GetMousePositionOnGameObject(terrain.gameObject);
		if(mouseOnTerrain == Vector3.zero)
		{
			return;
		}
		
		Vector3 towerPosition = GetTowerPosition(mouseOnTerrain);
		BuildSelectedTower(towerPosition);
	}
	
	private void OnMapActiveClick()
	{
		
	}
	
	private void OnClick()
	{
		switch(mapState)
		{
		case MapState.BUILDING_TOWER:
			OnTowerBuildingActionClick();
			break;
		case MapState.ACTIVE:
			OnMapActiveClick();
			break;
		}
	}
	
	// Use this for initialization
	void Start() 
	{
		OnValidate();
		HideGrid();
		//mouseClickHandler.isClickAccepted = IsClickAccepted;
		mouseClickHandler.onClick = OnClick;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
