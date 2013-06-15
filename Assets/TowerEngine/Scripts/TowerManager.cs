using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class TowerManager : MonoBehaviour, SavingGameComponent
{
	private static TowerManager instance;
		
	public enum MapState
	{
		ACTIVE,
		SELECTING_TOWER,
		UPGRADING_TOWER
	}
	
	[System.Serializable]
	private class TowerInfo
	{
		public Tower tower;
		public int id;
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
	
	public TextWithIcon goldBar;
	
	public ObjectLabel towerSelectionTexture;
	
	private MapState mapState = MapState.ACTIVE;
	public GameObject selectedTower;
	private int currentGold;
	private GuiEventsHandler towerBuildButtonHandler;
	private IDictionary<Vector2, TowerInfo> towerNameByPlaceMap = new Dictionary<Vector2, TowerInfo>();
	private MouseClickHandler mouseClickHandler = new MouseClickHandler();
	private Vector3 towerPosition;
	private TowersBar towersBarComponent;
	private TextureTrigger towerBuildButtonTrigger;
	private bool towersBarWasShown = false;
	private bool towerBuildButtonStateBeforeHide = false;
	private bool skillsBarClosed = false;
	private Tower lastClickedTower;
	private Vector3 towerSelectionTextureInitialOffset;
	private int currentGoldOnRoundStart;
	private TowerSaveData[] towersOnRoundStarted;
	private SaveData saveData;
	
	public static TowerManager Instance
	{
		get
		{
			return instance;
		}
	}
	
	public int CurrentGold
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
			towerSkillsBar.UpdateButtonsGoldState(currentGold);
		}
	}
	
	private void UpdateGoldBar()
	{
		if(goldBar != null)
		{
			goldBar.text = CurrentGold.ToString();
		}
	}
	
	private void InitTowersBarIfNeed()
	{
		if(towersBar == null || towersBarComponent != null)
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
		InitTowersBarIfNeed();
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
	
	private void NewTowerBuiltNotifyAllBut(Tower but)
	{
		foreach(TowerInfo towerInfo in towerNameByPlaceMap.Values)
		{
			Tower tower = towerInfo.tower;
			if(tower != but)
			{
				tower.NotifyNewTowerBuilt();
			}
		}
	}
	
	private void PutTower(Vector3 towerPosition, GameObject towerPrefab)
	{
		GameObject towerObject = PositionUtilities.InstantiateGameObjectAndPutCenterOnXZPlane(towerPrefab, towerPosition);
		Tower tower = towerObject.GetComponent<Tower>();
		TowerInfo towerInfo = new TowerInfo();
		towerInfo.id = towersBarComponent.GetTowerIdByPrefabObject(towerPrefab);
		towerInfo.tower = tower;
		towerNameByPlaceMap[new Vector2(towerPosition.x, towerPosition.z)] = towerInfo;
		NewTowerBuiltNotifyAllBut(tower);
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
			PutTower(towerPosition, towerPrefab);
			CurrentGold -= gold;
		}
	}
	
	private void BuildSelectedTower(Vector3 towerPosition)
	{
		BuildTower(towerPosition, selectedTower);
	}
	
	private void SetTowerBuildButtonState(bool value)
	{
		towerBuildButtonTrigger.ChangeTexture(value);
	}
	
	private void SetTowerBuildButtonToNormal()
	{
		SetTowerBuildButtonState(true);
	}
	
	private void SetTowerBuildButtonToCancel()
	{
		SetTowerBuildButtonState(false);
	}
	
	private void ReplaceTower(Vector3 towerPosition)
	{
		
	}
	
	private string GetTowerName(GameObject tower)
	{
		Tower towerComponent = tower.GetComponent<Tower>();
		return towerComponent.name;
	}
	
	private bool HandleTowerPlaceSelection()
	{
		TowerInfo towerInfo = null;
		
		if(!towerNameByPlaceMap.TryGetValue(new Vector3(towerPosition.x, towerPosition.z), out towerInfo))
		{
			BuildSelectedTower(towerPosition);
			return true;
		}
		else
		{
			string selectedTowerName = GetTowerName(selectedTower);
			string towerName = towerInfo.tower.name;
			if(towerName != selectedTowerName)
			{
				ReplaceTower(towerPosition);
			}
		}
		
		return false;
	}
	
	private void OnTowerPlaceClickWhenTowerNotSelected()
	{
		ShowMessage(Messenger.Instance.towerNotSelectedMessage);
	}
	
	private void AttachSelectionTextureToTower(Tower tower)
	{
		if(towerSelectionTexture == null)
		{
			return;
		}
		
		GameObject gameObject = tower.gameObject;
		towerSelectionTexture.target = gameObject.transform;
	}
	
	private void OnTowerClick(Tower tower)
	{
		towerSkillsBar.SetUpgrades(tower.upgrades);
		towerSkillsBar.UpdateButtonsGoldState(CurrentGold);
		lastClickedTower = tower;
		AttachSelectionTextureToTower(tower);
		ShowSkillsBar();
		towerBuildButton.SetActive(false);
	}
	
	private void CheckClickedTower()
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
	
	private void OnMapClick()
	{
		if(mapState == MapState.SELECTING_TOWER)
		{
			TowerPlaceSelectionResult towerPlaceSelectionResult = CheckTowerPlaceSelection();
			if(towerPlaceSelectionResult == TowerPlaceSelectionResult.TOWER_SELECTED)
			{
				CheckClickedTower();
				SetTowerBuildButtonToNormal();
				towerBuildButton.SetActive(false);
				HideTowersBar();
			}
			else if(towerPlaceSelectionResult == TowerPlaceSelectionResult.NONE)
			{
				Messenger.Instance.ShowMessage(Messenger.Instance.cantBuildHereMessage);
			}
		}
		else
		{
			CheckClickedTower();
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
		
		TowerInfo towerInfo = null;
		towerNameByPlaceMap.TryGetValue(towerPosition, out towerInfo);
		
		if(towerInfo == null)
		{
			return null;
		}
		
		return towerInfo.tower;
	}
	
	private Tower GetClickedTower()
	{
		RaycastHit towerPlace = GetTowerPlaceHit();
		return GetTowerByHit(towerPlace);
	}
	
	private enum TowerPlaceSelectionResult
	{
		NONE,
		TOWER_NOT_SELECTED,
		TOWER_PLACE_SELECTED,
		TOWER_SELECTED
	}
	
	private TowerPlaceSelectionResult CheckTowerPlaceSelection()
	{
		RaycastHit mouseOnGrid = GetTowerPlaceHit();
		if(mouseOnGrid.point == Vector3.zero)
		{
			return TowerPlaceSelectionResult.NONE;
		}
		
		if(selectedTower == null)
		{
			OnTowerPlaceClickWhenTowerNotSelected();
			return TowerPlaceSelectionResult.TOWER_NOT_SELECTED;
		}
		
		towerPosition = GetTowerPosition(mouseOnGrid);
		if(HandleTowerPlaceSelection())
		{
			return TowerPlaceSelectionResult.TOWER_PLACE_SELECTED;
		}
		else
		{
			return TowerPlaceSelectionResult.TOWER_SELECTED;
		}
	}
	
	private void CloseTowerBuildMenu()
	{
		mapState = MapState.ACTIVE;
		HideGrid();
		HideTowersBar();
		SetTowerBuildButtonToNormal(); 
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
		SetTowerBuildButtonToCancel();
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
	}
	
	private void ShowSkillsBar()
	{
		SetSkillsBarVisibility(true);
		HideGrid();
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
		if(towerNameByPlaceMap.Remove(position))
		{
			TowerDestroyedNotifyAll(tower);
			Destroy(tower.gameObject);
		}
	}

	private void TowerDestroyedNotifyAll(Tower destroyedTower)
	{
		foreach(TowerInfo towerInfo in towerNameByPlaceMap.Values)
		{
			Tower tower = towerInfo.tower;
			tower.NotifySomeTowerDestroyed(destroyedTower);
		}
	}
	
	private void SellTower(Tower tower)
	{
		CurrentGold += tower.goldPrice / 2;
		KickTower(tower);
		HideSkillsBar();
		towerBuildButton.SetActive(true);
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
		towerSkillsBar.gameObject.SetActive(false);
	}
	
	public void ShowAllControls()
	{
		if(towersBarWasShown)
		{
			ShowTowersBar();
		}
		
		towerBuildButtonTrigger.ChangeTexture(towerBuildButtonStateBeforeHide);
		
		if(mapState == MapState.UPGRADING_TOWER)
		{
			towerSkillsBar.gameObject.SetActive(true);
		}
		else
		{
			towerBuildButton.SetActive(true);
		}
	}
	
	public void NotifyNewRoundStarted()
	{
		UpdateSavingData();
	}
	
	private void UpdateSavingTowers()
	{
		int index = 0;
		towersOnRoundStarted = new TowerSaveData[towerNameByPlaceMap.Count];
		foreach(TowerInfo towerInfo in towerNameByPlaceMap.Values)
		{
			TowerSaveData towerSaveData = new TowerSaveData();
			towerSaveData.position = new SerializableVector3(towerInfo.tower.gameObject.transform.position);
			towerSaveData.id = towerInfo.id;
			towersOnRoundStarted[index] = towerSaveData;
			index++;
		}
	}
	
	private void UpdateSavingData()
	{
		currentGoldOnRoundStart = currentGold;
		UpdateSavingTowers();
	}
	
	// Use this for initialization
	void Start() 
	{
		InitTowersBarIfNeed();
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
		
		CurrentGold = startGold;
		
		if(towerSelectionTexture != null)
		{
			towerSelectionTextureInitialOffset = towerSelectionTexture.offset;
		}
		
		Restore();
		UpdateSavingData();
	}
	
	private void DrawCloseButton()
	{
		if(GameMenu.Instance.IsShown() || GameMenu.Instance.IsLoading())
		{
			return;
		}
		
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
		TowerInfo towerInfo = new TowerInfo();
		towerInfo.id = towersBarComponent.GetTowerIdByPrefabObject(replaceToObject);
		replaceTo = Utilities.InstantiateAndGetComponent<Tower>(replaceToObject.gameObject, position);
		towerInfo.tower = replaceTo;
		towerNameByPlaceMap[xy] = towerInfo;
		
		Destroy(tower.gameObject);
		OnTowerClick(replaceTo);
	}
	
	private void UpgradeTower(Tower upgradeTo, int cost)
	{
		CurrentGold -= cost;
		ReplaceTower(lastClickedTower, upgradeTo);
	}
	
	private void OnMaxUpgradeSelected()
	{
		Messenger.Instance.ShowMessage(Messenger.Instance.maxUpgradeSelectedMessage);
	}
	
	private void OnTowerUpgradeSelected(TowerSkillsBar.TowerUpgrade towerUpgrade)
	{
		if(towerUpgrade.tower != null)
		{
			if(towerUpgrade.goldCost <= currentGold)
			{
				UpgradeTower(towerUpgrade.tower, towerUpgrade.goldCost);
			}
			else
			{
				OnNotEnoughGold();
			}
		}
		else
		{
			OnMaxUpgradeSelected();
		}
	}
	
	private void OnClickWhileUpgradingTower(int index)
	{
		if(mapState == MapState.UPGRADING_TOWER)
		{	
			int buttonIndex = towerSkillsBar.GetClickedButtonIndex();
			TowerSkillsBar.TowerUpgrade towerUpgrade = towerSkillsBar.GetTowerUpgradeByIndex(buttonIndex);

			if(buttonIndex == towerSkillsBar.GetButtonsCount() - 1)
			{
				SellTower(lastClickedTower);
			}
			else if(buttonIndex >= 0)
			{
				if(towerUpgrade == null)
				{
					OnNoTowerUpgradeSelected();
				}
				else
				{
					OnTowerUpgradeSelected(towerUpgrade);
				}
			}		
		}
	}
	
	private void ClearTowerSelectionArrow()
	{
		if(mapState != MapState.UPGRADING_TOWER)
		{
			towerSelectionTexture.target = null;
		}
	}
	
	void OnGUI()
	{
		if(GameMenu.Instance.IsLoading())
		{
			goldBar.gameObject.SetActive(false);
			return;
		}
		
		if(mapState == MapState.UPGRADING_TOWER)
		{
			DrawCloseButton();
		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		UpdateClicks();
		ClearTowerSelectionArrow();
		
		if(skillsBarClosed)
		{
			HideSkillsBar();
			towerBuildButton.SetActive(true);
			skillsBarClosed = false;
			return;
		}
	}
	
	[System.Serializable]
	private class TowerSaveData
	{
		public int id;
		public int[] upgrades;
		public SerializableVector3 position;
	}
	
	[System.Serializable]
	private class SaveData
	{
		public TowerSaveData[] towers;
		public int gold;
	}
	
	private void RestoreTowers(TowerSaveData[] towers)
	{
		InitTowersBarIfNeed();
		
		foreach(TowerSaveData tower in towers)
		{
			Tower towerPrefab = towersBarComponent.GetTowerById(tower.id);
			PutTower(tower.position.ToVector3(), towerPrefab.gameObject);
		}
	}
	
	public void Restore()
	{
		if(saveData != null)
		{
			CurrentGold = saveData.gold;
			RestoreTowers(saveData.towers);
		}
	}
	
	public object OnSave()
	{
		SaveData saveData = new SaveData();
		saveData.gold = currentGoldOnRoundStart;
		saveData.towers = towersOnRoundStarted;
		return saveData;
	}
	
	public void OnRestore(object data)
	{
		saveData = (SaveData)data;
	}
}
