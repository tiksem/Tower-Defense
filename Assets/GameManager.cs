using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public int maxLeaveTargetCount = 50;
	private int leaveTargetCount = 0;
	
	public void NotifyTargetLeave()
	{
		leaveTargetCount++;
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
