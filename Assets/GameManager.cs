using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class GameManager : MonoBehaviour, SavingGameComponent
{
	public static GameManager instance;
	
	[System.Serializable]
	public class Message
	{
		public string text = "";
		public float delayBeforeShow = 0.0f;
	}
	
	[System.Serializable]
	public class LevelDescriptionMessage
	{
		public float delayBeforeShow = 0.0f;
		public int levelOffset = 0;
	}
	
	public enum PartyPointOrientation
	{
		HORIZONTAL,
		VERTICAL
	}
	
	[System.Serializable]
	public class Party
	{
		public GameObject targetPrefab;
		public GameObject leaderPrefab;
		public int width = 5;
		public int height = 5;
		public int gold = 30;
		
		public Message[] endRoundMessages;
		public LevelDescriptionMessage[] endRoundLevelDescriptions = new LevelDescriptionMessage[1];
	}
	
	public int maxLeaveTargetCount = 50;
	public Party[] parties;
	public string[] wayPointTags = new string[]{"WayPoint1", "WayPoint2"};
	public float defaultTargetRadius = 0.5f;
	public string wonText = "You won!!!";
	public string looseText = "You loose!!!";
	public string freeWonText = "You won, wanna more levels?";
	public Texture wonBackground;
	public Texture looseBackground;
	public GUITimer roundTimerPrefab;
	public GUIText levelsBar;
	
	public float unitDisappearFadeDuration = 1.0f;
	public float unitAppearFadeDuration = 1.0f;
	public float durationBeforeLoose = 2.0f;
	
	public Transform portalAppearancePosition;
	public GameObject portal;
	public float delayBetweenPartyAndPortalCreation = 2.0f;
	public Vector3 leavePortalOffset = new Vector3(0.0f, 3.0f, 0.0f);
	
	public PartyPointOrientation partyPointOrientation = PartyPointOrientation.HORIZONTAL;
	
	public TextWithIcon lifesBar;
	
	public int partyIndex = 0;
	
	public ObstacleAvoidanceType obstaclesTargetAvoidanceQuailty = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
	
	public bool isFree = false;
	
	private int leaveTargetCount = 0;
	private int leavesCountOnRoundStart = 0;
	
	private bool win = false;
	private bool loose = false;
	
	private bool adsDisaplyed = false;
	
	private GameObject[] partyAppearingPoints;
	
	private GameObject createdPortal;
	
	private bool roundTimerFired = false;
	
	private object restoreData;
	
	private int LeaveTargetCount
	{
		get
		{
			return leaveTargetCount;
		}
		set
		{
			leaveTargetCount = value;
			UpdateLifesBar();
		}
	}
	
	void OnValidate()
	{
		foreach(Party party in parties)
		{
			/*int levelDescriptionsCount = party.endRoundLevelDescriptions.Length;
			if(levelDescriptionsCount == 0)
			{
				party.endRoundLevelDescriptions = new GameManager.LevelDescriptionMessage[1];
			}*/
			
			NavMeshAgent targetNavMeshAgent = party.targetPrefab.GetComponent<NavMeshAgent>();
			targetNavMeshAgent.obstacleAvoidanceType = obstaclesTargetAvoidanceQuailty;
		}
		
		
	}
	
	private IEnumerator LooseAction()
	{
		yield return new WaitForSeconds(durationBeforeLoose);
		OnLoose();
		loose = true;
	}
	
	private void UpdateLeaveTargetsState()
	{
		if(LeaveTargetCount >= maxLeaveTargetCount)
		{
			StartCoroutine(LooseAction());
		}
	}
	
	private IEnumerator FadeTargetAppearance(GameObject target)
	{
		return Animations.CreateFadeDownOrUp(target, unitAppearFadeDuration);
	}
	
	private IEnumerator FadeTargetDisappearanceCoroutine(GameObject target)
	{
		return Animations.CreateFadeDownOrUp(target, unitDisappearFadeDuration);
	}
	
	private IEnumerator FadeTargetDisappearance(WayPointFollower target)
	{
		GameObject portal = CreateLeavePortal(target);
		yield return StartCoroutine(FadeTargetDisappearanceCoroutine(target.gameObject));
		
		if(target != null)
		{
			LeaveTargetCount++;
			UpdateLeaveTargetsState();
			Destroy(target.gameObject);
		}
		
		Destroy(portal);
	}
	
	private GameObject CreateLeavePortal(WayPointFollower target)
	{
		Vector3 position = target.GetLastWayPoint();
		position += leavePortalOffset;
		return (GameObject)Instantiate(portal, position, portal.transform.rotation);
	}
	
	private void NotifyTargetLeave(WayPointFollower target)
	{
		StartCoroutine(FadeTargetDisappearance(target));
	}
	
	private void CreatePortal()
	{
		if(portal == null || portalAppearancePosition == null)
		{
			return;
		}
		
		createdPortal = (GameObject)Instantiate(portal, portalAppearancePosition.position, portal.transform.rotation);
	}
	
	private IEnumerator PortalCreationAction(float delay)
	{
		yield return new WaitForSeconds(delay);
		CreatePortal();
	}
	
	private void UpdateLifesBar()
	{
		if(lifesBar != null)
		{
			int lifes = maxLeaveTargetCount - LeaveTargetCount;
			if(lifes < 0)
			{
				lifes = 0;
			}
			
			lifesBar.IntText = lifes;
		}
	}
	
	private void RecvGoldFromParty()
	{
		if(partyIndex > 0 && partyIndex <= parties.Length)
		{
			Party party = parties[partyIndex - 1];
			TowerManager.Instance.CurrentGold += party.gold;
		}
	}
	
	private string GetMessageTextForLevel(int level, Target target)
	{
		string armorType = target.GetArmorTypeAsString();
		string type = target.GetTypeAsString();
		return string.Format(Messenger.Instance.nextLevelDescriptionPattern, level, type, armorType);
	}
	
	private IEnumerator ShowLevelDescriptionMessages()
	{
		Party party = parties[partyIndex];
		LevelDescriptionMessage[] messages = party.endRoundLevelDescriptions;
		foreach(LevelDescriptionMessage message in messages)
		{
			yield return new WaitForSeconds(message.delayBeforeShow);
			int level = partyIndex + 1 + message.levelOffset;
			party = parties[level - 1];
			Target target = party.targetPrefab.GetComponent<Target>();
			string text = GetMessageTextForLevel(level, target);
			Messenger.Instance.ShowMessage(text);
		}
	}
	
	private IEnumerator ShowEndRoundMessages()
	{
		Party party = parties[partyIndex];
		Message[] messages = party.endRoundMessages;
		foreach(Message message in messages)
		{
			yield return new WaitForSeconds(message.delayBeforeShow);
			Messenger.Instance.ShowMessage(message.text);
		}
	}
	
	private void OnAllTargetsDestroyed()
	{
		if(roundTimerPrefab != null)
		{
			partyIndex++;
			if(partyIndex >= parties.Length)
			{
				OnWin();
				win = true;
				return;
			}
			else
			{
				TowerManager.Instance.NotifyNewRoundStarted();
				StartCoroutine(ShowEndRoundMessages());
				StartCoroutine(ShowLevelDescriptionMessages());
				UpdateLevelIndex();
			}
			
			GUITimer timer = GUITimer.CreateFromPrefab(roundTimerPrefab, NextParty);
			float portalCreationDelay = timer.seconds - delayBetweenPartyAndPortalCreation;
			StartCoroutine(PortalCreationAction(portalCreationDelay));
			roundTimerFired = true;
			
			RecvGoldFromParty();
		}
		else
		{
			NextParty();
		}
		
		leavesCountOnRoundStart = LeaveTargetCount;
	}
	
	private bool AllTargetsDestroyed()
	{
		GameObject[] targets = Weapon.GetAvailibleTargets();
		return targets.Length <= 0;
	}
	
	private float GetTargetWidth(GameObject target)
	{
		NavMeshAgent navMeshAgent = target.GetComponent<NavMeshAgent>();
		if(navMeshAgent != null)
		{
			return navMeshAgent.radius;
		}
		else
		{
			return defaultTargetRadius;
		}
	}
	
	private void AssignTargetWayPointTag(int partyPointIndex, GameObject target)
	{
		WayPointFollower wayPointFollower = target.GetComponent<WayPointFollower>();
		if(wayPointFollower == null)
		{
			throw new System.ArgumentException("target must have WayPointFollower component");
		}
		
		string tag = wayPointTags[partyPointIndex];
		wayPointFollower.SetWayPointTag(tag);
		wayPointFollower.onFinish = NotifyTargetLeave;
	}
	
	private GameObject CreatePartyUnit(int partyPointIndex, GameObject prefab, Vector3 position)
	{
		GameObject target = (GameObject)Instantiate(prefab, position, prefab.transform.rotation);
		Rendering.SetAlpha(target, 0.0f);
		StartCoroutine(FadeTargetAppearance(target));
		
		AssignTargetWayPointTag(partyPointIndex, target);
		
		//target.transform.position = position;
		
		/*WayPointFollower wayPointFollower = target.GetComponent<WayPointFollower>();
		if(wayPointFollower != null)
		{
			wayPointFollower.wayPointOffsetX = party.height + targetWidth * x * 2;
			wayPointFollower.wayPointOffsetY = party.height + targetWidth * y * 2;
		}*/
		
		return target;
	}
	
	private GameObject CreatePartyTarget(Party party, int partyPointIndex, int x, int y)
	{
		NavMeshAgent navMeshAgent = party.targetPrefab.GetComponent<NavMeshAgent>();
		float targetWidth = GetTargetWidth(party.targetPrefab);
		GameObject partyPositionPoint = partyAppearingPoints[partyPointIndex];
		
		Vector3 position = partyPositionPoint.transform.position;
		position.x += targetWidth * x;
		position.z += targetWidth * y;
		
		return CreatePartyUnit(partyPointIndex, party.targetPrefab, position);
	}
	
	private GameObject CreatePartyLeader(int partyPointIndex, Vector3 position, Party party)
	{
		if(party.leaderPrefab == null)
		{
			return null;
		}
		
		position.x += GetTargetWidth(party.leaderPrefab);
		return CreatePartyUnit(partyPointIndex, party.leaderPrefab, position);
	}
	
	private void CreateParty(int partyPointIndex)
	{
		Party partyToCreate = parties[partyIndex];
		GameObject partyPositionPoint = partyAppearingPoints[partyPointIndex];
		Vector3 leaderPosition = partyPositionPoint.transform.position;
		
		float zInFirstLine = 0.0f;
		float zInLastLine = 0.0f;
		
		int partyWidth = partyToCreate.width;
		int partyHeight = partyToCreate.height;
		
		if(partyPointOrientation == PartyPointOrientation.VERTICAL)
		{
			partyWidth = partyToCreate.height;
			partyHeight = partyToCreate.width;
		}
		
		for(int y = 0; y < partyHeight; y++)
		{
			GameObject target = null;
			
			for(int x = 0; x < partyWidth; x++)
			{
				target = CreatePartyTarget(partyToCreate, partyPointIndex, x, y);
				if(x == partyToCreate.width - 1 && y == 0)
				{
					leaderPosition.x = target.transform.position.x;
					leaderPosition.x += GetTargetWidth(target);
				}
			}
			
			if(y == 0)
			{
				zInFirstLine = target.transform.position.z;
			}
			else if(y == partyToCreate.height - 1)
			{
				zInLastLine = target.transform.position.z;
			}
		}
		
		leaderPosition.z = zInFirstLine + (zInLastLine - zInFirstLine) / 2;
		CreatePartyLeader(partyPointIndex, leaderPosition, partyToCreate);
	}
	
	private void CreateParties()
	{
		for(int i = 0; i < partyAppearingPoints.Length; i++)
		{
			CreateParty(i);
		}
	}
	
	private void UpdateLevelIndex()
	{
		if(levelsBar != null)
		{
			levelsBar.text = (partyIndex + 1) + " level";
		}
	}
	
	private void NextParty()
	{
		if(createdPortal != null)
		{
			Destroy(createdPortal);
		}
		
		UpdateLevelIndex();
		roundTimerFired = false;
		CreateParties();
	}
	
	private void DrawWinBackground()
	{
		GUIUtilities.DrawBackground(wonBackground);
	}
	
	private void DrawLooseBackground()
	{
		GUIUtilities.DrawBackground(looseBackground);
	}
	
	private string GetEndGameText()
	{
		string text = wonText;
		if(loose)
		{
			text = looseText;
		}
		else if(isFree)
		{
			text = freeWonText;
		}
		
		return text;
	}
	
	private void DrawEndGameGUIIfRequired()
	{
		if(!win && !loose)
		{
			adsDisaplyed = false;
			return;
		}
		
		GameMenu.Instance.HideAllControls();
		DrawWinBackground();
		
		string text = GetEndGameText();
		
		GUIUtilities.MessageBoxType messageBoxType = GUIUtilities.MessageBoxType.OK;
		if(win && isFree)
		{
			messageBoxType = GUIUtilities.MessageBoxType.YES_NO;
		}
		
		GUIUtilities.MessageBoxResult messageBoxResult = GUIUtilities.DrawMessageBox(text, messageBoxType);
		
		if(messageBoxResult == GUIUtilities.MessageBoxResult.YES)
		{
			MainMenu.OpenProAppliactionOnPlayStore();
		}
		
		if(messageBoxResult != GUIUtilities.MessageBoxResult.NONE)
		{
			GameMenu.Instance.EndGame();
		}
		
		if(isFree && !adsDisaplyed)
		{
			AdManager.instance.ShowEndOfTheRoundAds();
			adsDisaplyed = true;
		}
	}
	
	private void OnWin()
	{
		GameMenu.Instance.PauseGame();
	}
	
	private void OnLoose()
	{
		GameMenu.Instance.PauseGame();
	}
	
	private void InitPartiesPoints()
	{
		partyAppearingPoints = GameObject.FindGameObjectsWithTag("PartyPoint");
	}
	
	void Awake()
	{
		if(instance != null)
		{
			throw new System.ApplicationException("GameManager can be created only once");
		}
		
		instance = this;
		
		maxLeaveTargetCount = MainMenu.GetLifesCount();
	}
	
	void Start() 
	{
		InitPartiesPoints();
		partyIndex--;
		LeaveTargetCount = 0;
	}
	
		
	private void KillAllTargets()
	{
		GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
		foreach(GameObject gameObject in targets)
		{
			Target target = gameObject.GetComponent<Target>();
			target.Damage(Weapon.AttackType.NORMAL, 9999999);
		}
	}
	
	void Update()
	{
		if(Input.GetKeyUp(KeyCode.K))
		{
			KillAllTargets();
		}
	}
	
	void FixedUpdate() 
	{
		if(win || loose)
		{
			return;
		}
		
		if(!roundTimerFired && AllTargetsDestroyed())
		{
			OnAllTargetsDestroyed();
		}
	}
	
	void OnGUI()
	{
		if(GameMenu.Instance.IsLoading())
		{
			lifesBar.gameObject.SetActive(false);
			return;
		}
		
		DrawEndGameGUIIfRequired();
	}
	
	[System.Serializable]
	class SaveData
	{
		public int partyIndex;
		public int leavesCount;
	}
	
	public void OnRestore(object data)
	{
		if(data != null)
		{
			SaveData saveData = (SaveData)data;
			partyIndex = saveData.partyIndex;
			LeaveTargetCount = saveData.leavesCount;
		}
	}
	
	public object OnSave()
	{
		SaveData saveData = new SaveData();
		saveData.partyIndex = partyIndex;
		saveData.leavesCount = leavesCountOnRoundStart;
		return saveData;
	}
}
