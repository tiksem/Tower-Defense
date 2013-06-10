using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class GameManager : MonoBehaviour, SavingGameComponent
{
	public static GameManager instance;
	
	[System.Serializable]
	public class Party
	{
		public GameObject targetPrefab;
		public GameObject leaderPrefab;
		public int width = 5;
		public int height = 5;
		public int gold = 30;
	}
	
	public int maxLeaveTargetCount = 50;
	public Party[] parties;
	public string[] wayPointTags = new string[]{"WayPoint1", "WayPoint2"};
	public float defaultTargetRadius = 0.5f;
	public string wonText = "You won!!!";
	public string looseText = "You loose!!!";
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
	
	public TextWithIcon lifesBar;
	
	private int partyIndex = -1;
	
	private int leaveTargetCount = 0;
	private int leavesCountOnRoundStart = 0;
	
	private bool win = false;
	private bool loose = false;
	
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
		if(partyIndex >= 0 && partyIndex < parties.Length)
		{
			Party party = parties[partyIndex];
			TowerManager.Instance.CurrentGold += party.gold;
		}
	}
	
	private void OnAllTargetsDestroyed()
	{
		RecvGoldFromParty();
		
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
				UpdateLevelIndex();
			}
			
			GUITimer timer = GUITimer.CreateFromPrefab(roundTimerPrefab, NextParty);
			float portalCreationDelay = timer.seconds - delayBetweenPartyAndPortalCreation;
			StartCoroutine(PortalCreationAction(portalCreationDelay));
			roundTimerFired = true;
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
	
	private GameObject CreatePartyTarget(Party party, int partyPointIndex, int x, int y)
	{
		NavMeshAgent navMeshAgent = party.targetPrefab.GetComponent<NavMeshAgent>();
		float targetWidth = GetTargetWidth(party.targetPrefab);
		GameObject partyPositionPoint = partyAppearingPoints[partyPointIndex];
		
		Vector3 position = partyPositionPoint.transform.position;
		position.x += targetWidth * x;
		position.z += targetWidth * y;
		
		GameObject target = (GameObject)Instantiate(party.targetPrefab, position, party.targetPrefab.transform.rotation);
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
	
	private void CreateParty(int partyPointIndex)
	{
		Party partyToCreate = parties[partyIndex];
		for(int y = 0; y < partyToCreate.height; y++)
		{
			for(int x = 0; x < partyToCreate.width; x++)
			{
				CreatePartyTarget(partyToCreate, partyPointIndex, x, y);
			}
		}
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
	
	private void DrawLooseGUI()
	{
		GameMenu.Instance.HideAllControls();
		DrawWinBackground();
		if(GUIUtilities.DrawMessageBox(looseText, GUIUtilities.MessageBoxType.OK) == GUIUtilities.MessageBoxResult.OK)
		{
			GameMenu.Instance.EndGame();
		}
	}
	
	private void DrawWinGUI()
	{
		GameMenu.Instance.HideAllControls();
		DrawWinBackground();
		if(GUIUtilities.DrawMessageBox(wonText,GUIUtilities.MessageBoxType.OK) == GUIUtilities.MessageBoxResult.OK)
		{
			GameMenu.Instance.EndGame();
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
	
	void Start() 
	{
		if(instance != null)
		{
			throw new System.ApplicationException("GameManager can be created only once");
		}
		
		instance = this;
		
		InitPartiesPoints();
		LeaveTargetCount = 0;
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
			return;
		}
		
		if(win)
		{
			DrawWinGUI();
		}
		else if(loose)
		{
			DrawLooseGUI();
		}
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
			partyIndex = saveData.partyIndex - 1;
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
