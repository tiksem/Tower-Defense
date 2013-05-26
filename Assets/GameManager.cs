using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class GameManager : MonoBehaviour 
{
	public static GameManager instance;
	
	[System.Serializable]
	public class Party
	{
		public GameObject targetPrefab;
		public GameObject leaderPrefab;
		public int width = 5;
		public int height = 5;
	}
	
	public int maxLeaveTargetCount = 50;
	public Party[] parties;
	public string[] wayPointTags = new string[]{"WayPoint1", "WayPoint2"};
	public float defaultTargetRadius = 0.5f;
	public string wonText = "You won!!!";
	public Texture wonBackground;
	
	private int partyIndex = -1;
	
	private int leaveTargetCount = 0;
	
	private bool win = false;
	
	private GameObject[] partyAppearingPoints;
	
	private void NotifyTargetLeave(WayPointFollower target)
	{
		leaveTargetCount++;
	}
	
	private void OnAllTargetsDestroyed()
	{
		NextParty();
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
	
	private void NextParty()
	{
		partyIndex++;
		if(partyIndex >= parties.Length)
		{
			OnWin();
			win = true;
			return;
		}
		
		CreateParties();
	}
	
	private void DrawWinBackground()
	{
		GUIUtilities.DrawBackground(wonBackground);
	}
	
	private void DrawWinGUI()
	{
		GameMenu.Instance.HideAllControls();
		DrawWinBackground();
		if(GUIUtilities.DrawMessageBox(wonText,GUIUtilities.MessageBoxType.OK) == GUIUtilities.MessageBoxResult.OK)
		{
			GameMenu.Instance.EndGame();
			win = false;
		}
	}
	
	private void OnWin()
	{
		GameMenu.Instance.PauseGame();
	}
	
	private void InitPartiesPoints()
	{
		partyAppearingPoints = GameObject.FindGameObjectsWithTag("PartyPoint");
	}
	
	// Use this for initialization
	void Start () 
	{
		if(instance != null)
		{
			throw new System.ApplicationException("GameManager can be created only once");
		}
		
		instance = this;
		
		InitPartiesPoints();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(win)
		{
			return;
		}
		
		if(AllTargetsDestroyed())
		{
			OnAllTargetsDestroyed();
		}
	}
	
	void OnGUI()
	{
		if(win)
		{
			DrawWinGUI();
		}
	}
}
