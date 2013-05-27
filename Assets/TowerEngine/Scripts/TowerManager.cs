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
		UPGRADING_TOWER
	}
	
	public int startGold = 200;
	public GameObject[] availibleTowerPlaces;
	public float towerSize = 3;
	public Terrain terrain;
	public GameObject towerBuildButton;
	public TowerSkillsBar towerSkillsBar;
	public GameObject towersBar;
	public Texture towerSkillsBarClose;
	public float towerSkillsBarCloseSize = 0.1f;
	public float towerSkillsBarCloseBorder = 0.1f;
	
	public GameObject goldBar;
	
	private MapState mapState = MapState.ACTIVE;
	public GameObject selectedTower;
	private int currentGold;
	private GuiEventsHandler towerBuildButtonHandler;
	private IDictionary<Vector2, Tower> towerNameByPlaceMap = new Dictionary<Vector2, Tower>();
	private MouseClickHandler mouseClickHandler = new MouseClickHandler();
	private Vector3 towerPosition;
	private TowersBar towersBarComponent;
	private TextureTrigger towerBuildButtonTrigger;
	private GUIText goldText;
	private bool towersBarWasShown = false;
	private bool towerBuildButtonStateBeforeHide = false;
	private bool skillsBarClosed = false;
	private Tower lastClickedTower;
	
	public static TowerManager Instance
	{
		get
		{
			return instance;
		}
	}
	
	private int CurrentGold
	{
		get
		{
			return currentGold;
		}
		
		set
		{
			currentGold = value;
			UpdateGoldBar();
			towersBarComponent.UpdateTowersGoldState(currentGold);
		}
	}
	
	private void UpdateGoldBar()
	{
		if(goldText != null)
		{
			goldText.text = CurrentGold.ToString();
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
	
	private Vector2 GetTowerXY(RaycastHit raycastHit)
	{
		Vector3 position = GetTowerPosition(raycastHit);
		return new Vector2(position.x, position.z);
	}
	
	public void NotifyTargetDestroyed(Target target)
	{
		CurrentGold += target.goldForKill;
	}
	
	private void SetGridVisibility(bool value)
	{
		foreach(GameObject towerPlace in availibleTowerPlaces)
		{
			Rendering.SetRenderingEnabled(towerPlace, value);
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
	
	private void ShowMessage(string message)
	{
		if(Messenger.Instance != null)
		{
			Messenger.Instance.ShowMessage(message);
		}
	}
	
	private void OnNotEnoughGold()
	{
		ShowMessage(Messenger.Instance.notEnoughGoldMessage);
	}
	
	private void BuildTower(Vector3 towerPosition, GameObject towerPrefab)
	{
		int gold = towersBarComponent.TryBuySelectedTower(CurrentGold);
		if(gold < 0)
		{
			OnNotEnoughGold();
		}
		else
		{
			GameObject tower = PositionUtilities.InstantiateGameObjectAndPutCenterOnXZPlane(towerPrefab, towerPosition);
			towerNameByPlaceMap[new Vector2(towerPosition.x, towerPosition.z)] = tower.GetComponent<Tower>();
			CurrentGold -= gold;
		}
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
		Tower tower = null;
		
		if(!towerNameByPlaceMap.TryGetValue(new Vector3(towerPosition.x, towerPosition.z), out tower))
		{
			BuildSelectedTower(towerPosition);
		}
		else
		{
			string selectedTowerName = GetTowerName(selectedTower);
			string towerName = tower.name;
			if(towerName != selectedTowerName)
			{
				ReplaceTower(towerPosition);
			}
		}
	}
	
	private void OnTowerPlaceClickWhenTowerNotSelected()
	{
		ShowMessage(Messenger.Instance.towerNotSelectedMessage);
	}
	
	private void OnTowerClick(Tower tower)
	{
		towerSkillsBar.SetUpgrades(tower.upgrades);
		lastClickedTower = tower;
		ShowSkillsBar();
	}
	
	private void OnMapClick()
	{
		if(mapState == MapState.SELECTING_TOWER)
		{
			CheckTowerPlaceSelection();
		}
		else if(mapState == MapState.ACTIVE)
		{
			if(towerSkillsBar == null)
			{
				return;
			}
			
			Tower tower = GetClickedTower();
			if(tower != null)
			{
				OnTowerClick(tower);
			}
		}
	}
	
	private RaycastHit GetTowerPlaceHit()
	{
		return MouseUtilities.FindFirstGameObjectHitInMouseRay(availibleTowerPlaces);
	}
	
	private Tower GetTowerByHit(RaycastHit raycastHit)
	{
		Vector3 position = raycastHit.point;
		if(position == Vector3.zero)
		{
			return null;
		}
		
		Vector2 towerPosition = GetTowerXY(raycastHit);
		
		Tower tower = null;
		towerNameByPlaceMap.TryGetValue(towerPosition, out tower);
		return tower;
	}
	
	private Tower GetClickedTower()
	{
		RaycastHit towerPlace = GetTowerPlaceHit();
		return GetTowerByHit(towerPlace);
	}
	
	private bool CheckTowerPlaceSelection()
	{
		RaycastHit mouseOnGrid = GetTowerPlaceHit();
		if(mouseOnGrid.point == Vector3.zero)
		{
			return false;
		}
		
		if(selectedTower == null)
		{
			OnTowerPlaceClickWhenTowerNotSelected();
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
		ShowGrid();
	}
	
	private void UpdateSelectedTower()
	{
		selectedTower = towersBarComponent.GetSelectedTower();
	}
	
	private void SetSkillsBarVisibility(bool value)
	{
		mapState = value ? MapState.UPGRADING_TOWER : MapState.ACTIVE;
		towerSkillsBar.gameObject.SetActive(value);
		towerBuildButton.SetActive(!value);
	}
	
	private void ShowSkillsBar()
	{
		SetSkillsBarVisibility(true);
	}
	
	private void HideSkillsBar()
	{
		SetSkillsBarVisibility(false);
	}
	
	private void OnNoTowerUpgradeSelected()
	{
		Messenger.Instance.ShowMessage(Messenger.Instance.selectTowerUpgradeMessage);
	}
	
	private void KickTower(Tower tower)
	{
		Vector2 position = PositionUtilities.XYZToXZ(tower.gameObject);
		towerNameByPlaceMap.Remove(position);
		Destroy(tower.gameObject);
	}
	
	private void SellTower(Tower tower)
	{
		CurrentGold += tower.goldPrice / 2;
		KickTower(tower);
		HideSkillsBar();
	}
	
	private void UpdateClicks()
	{
		if(towerBuildButtonHandler.Update() != GuiEventsHandler.State.NONE)
		{
			return;
		}
		
		if(!GUIManager.Instance.GetMouseOverFlag())
		{
			mouseClickHandler.Update();
		}
	}
	
	public void HideAllControls()
	{
		towersBarWasShown = towersBar.activeSelf;
		HideTowersBar();
		towerBuildButtonStateBeforeHide = towerBuildButtonTrigger.GetState();
		towerBuildButton.SetActive(false);
	}
	
	public void ShowAllControls()
	{
		if(towersBarWasShown)
		{
			ShowTowersBar();
		}
		towerBuildButton.SetActive(true);
		towerBuildButtonTrigger.ChangeTexture(towerBuildButtonStateBeforeHide);
	}
	
	// Use this for initialization
	void Start() 
	{
		InitTowersBar();
		HideGrid();
		HideTowersBar();
		HideSkillsBar();
		towerBuildButtonHandler = new GuiEventsHandler(towerBuildButton);
		towerBuildButtonHandler.onMouseClick = OnTowerBuildButtonClick;
		//mouseClickHandler.isClickAccepted = IsMouseClickAccepted;
		mouseClickHandler.onClick = OnMapClick;
		towersBarComponent.GetComponent<BarWithCircleButtons>().onButtonClick = OnClickWhileSelectingTower;
		towerSkillsBar.GetComponent<BarWithCircleButtons>().onButtonClick = OnClickWhileUpgradingTower;
		towerBuildButtonTrigger = towerBuildButton.GetComponent<TextureTrigger>();
		
		if(goldBar != null)
		{
			goldText = goldBar.GetComponent<GUIText>();
		}
		
		CurrentGold = startGold;
	}
	
	private void DrawCloseButton()
	{
		if(GUIUtilities.DrawSquareButtonInRightTopCorner(towerSkillsBarClose, towerSkillsBarCloseSize, towerSkillsBarCloseBorder))
		{
			skillsBarClosed = true;
		}
	}
	
	private void OnClickWhileSelectingTower(int index)
	{
		if(mapState == MapState.SELECTING_TOWER)
		{
			if(towersBarComponent.UpdateEvents())
			{
				UpdateSelectedTower();
				return;
			}
		}
	}
	
	private void ReplaceTower(Tower tower, Tower replaceTo)
	{
		Vector3 position = tower.transform.position;
		Vector2 xy = PositionUtilities.XYZToXZ(position);
		
		GameObject replaceToObject = replaceTo.gameObject;
		replaceTo = Utilities.InstantiateAndGetComponent<Tower>(replaceToObject.gameObject, position);
		towerNameByPlaceMap[xy] = replaceTo;
		
		Destroy(tower.gameObject);
		OnTowerClick(replaceTo);
	}
	
	private void UpgradeTower(Tower upgradeTo, int cost)
	{
		CurrentGold -= cost;
		ReplaceTower(lastClickedTower, upgradeTo);
	}
	
	private void OnTowerUpgradeSelected(TowerSkillsBar.TowerUpgrade towerUpgrade)
	{
		if(towerUpgrade.tower != null)
		{
			UpgradeTower(towerUpgrade.tower, towerUpgrade.goldCost);
		}
	}
	
	private void OnClickWhileUpgradingTower(int index)
	{
		if(mapState == MapState.UPGRADING_TOWER)
		{	
			int buttonIndex = towerSkillsBar.GetClickedButtonIndex();
			TowerSkillsBar.TowerUpgrade towerUpgrade = towerSkillsBar.GetTowerUpgradeByIndex(buttonIndex);
			
			if(buttonIndex >= 0 && towerUpgrade == null)
			{
				OnNoTowerUpgradeSelected();
			}
			else if(buttonIndex == towerSkillsBar.GetButtonsCount() - 1)
			{
				SellTower(lastClickedTower);
			}
			else if(buttonIndex >= 0)
			{
				OnTowerUpgradeSelected(towerUpgrade);
			}
		}
	}
	
	void OnGUI()
	{
		if(mapState == MapState.UPGRADING_TOWER)
		{
			DrawCloseButton();
		}
	}
	
	// Update is called once per frame
	void LateUpdate() 
	{
		UpdateClicks();
		
		if(skillsBarClosed)
		{
			HideSkillsBar();
			skillsBarClosed = false;
			return;
		}
	}
}
