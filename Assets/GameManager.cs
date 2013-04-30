using UnityEngine;
using System.Collections;

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
	
	private int partyIndex = -1;
	
	private int leaveTargetCount = 0;
	
	private bool win = false;
	
	public void NotifyTargetLeave()
	{
		leaveTargetCount++;
	}
	
	private void OnAllTargetsDestroyed()
	{
		NextParty();
	}
	
	private bool AllTargetsDestroyed()
	{
		GameObject[] targets = Weapon.GetvailibleTargets();
		return targets.Length <= 0;
	}
	
	private float GetTargetWidth(GameObject target)
	{
		CharacterController characterController = target.GetComponent<CharacterController>();
		return characterController.radius;
	}
	
	private GameObject CreatePartyTarget(Party party, int x, int y)
	{
		GameObject target = (GameObject)Instantiate(party.targetPrefab);
		float targetWidth = GetTargetWidth(target);
		
		Vector3 position = transform.position;
		position.x += targetWidth * x;
		position.z += targetWidth * y;
		
		target.transform.position = position;
		//target.transform.rotation = transform.rotation;
		
		return target;
	}
	
	private void CreateParty()
	{
		Party partyToCreate = parties[partyIndex];
		for(int y = 0; y < partyToCreate.height; y++)
		{
			for(int x = 0; x < partyToCreate.width; x++)
			{
				CreatePartyTarget(partyToCreate, x, y);
			}
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
		
		CreateParty();
	}
	
	private void OnWin()
	{
		
	}
	
	// Use this for initialization
	void Start () 
	{
		if(instance != null)
		{
			throw new System.ApplicationException("GameManager can be created only once");
		}
		
		instance = this;
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
}
