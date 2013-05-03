using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(NavMeshAgent))]
public class WayPointFollower : MonoBehaviour 
{
	private String wayPointTag;
	
	public float wayPointRadius = 10.0f;
	
	private int currentWayPointIndex = -1;
	private Vector3[] wayPoints;
	private NavMeshAgent navMeshAgent;
	private bool finishGotten = false;
	private float sqrWayPointRadius;
	
	void OnValidate()
	{
		sqrWayPointRadius = wayPointRadius * wayPointRadius;
	}
	
	private void InitNavMeshAgentIfNeed()
	{
		if(navMeshAgent != null)
		{
			return;
		}
		
		navMeshAgent = GetComponent<NavMeshAgent>();
		navMeshAgent.stoppingDistance = 5.0f;
	}
	
	private void InitWayPoints()
	{
		GameObject[] wayPointObjects = GameObject.FindGameObjectsWithTag(wayPointTag);
		Array.Sort(wayPointObjects, (GameObject a, GameObject b) => 
		{
			int intA = int.Parse(a.name);
			int intB = int.Parse(b.name);
			return intA.CompareTo(intB);
		});
		
		wayPoints = new Vector3[wayPointObjects.Length];
		
		for(int i = 0; i < wayPointObjects.Length; i++)
		{
			wayPoints[i] = wayPointObjects[i].transform.position;
		}
	}
	
	public void SetWayPointTag(string wayPointTag)
	{
		this.wayPointTag = wayPointTag;
		currentWayPointIndex = -1;
		InitNavMeshAgentIfNeed();
		InitWayPoints();
		MoveToNextWayPoint();
	}
	
	public void Start()
	{
		
	}
	
	private void OnWayPointGotten()
	{
		MoveToNextWayPoint();
	}
	
	private void OnFinish()
	{
		finishGotten = true;
	}
	
	private Vector3 GetCurrentWayPoint()
	{
		return wayPoints[currentWayPointIndex];
	}
	
	private bool IsWayPointGotten()
	{
		Vector3 direction = navMeshAgent.destination - transform.position;
		return direction.sqrMagnitude <= sqrWayPointRadius;
	}
	
	private void MoveToWayPoint(Vector3 wayPoint)
	{
		navMeshAgent.destination = wayPoint;
	}
	
	private void MoveToNextWayPoint()
	{
		currentWayPointIndex++;
		if(currentWayPointIndex >= wayPoints.Length)
		{
			OnFinish();
		}
		else
		{
			Vector3 wayPoint = wayPoints[currentWayPointIndex];
			MoveToWayPoint(wayPoint);
		}
	}
	
	void Update()
	{
		if(wayPointTag == null)
		{
			return;
		}
		
		if(finishGotten)
		{
			return;
		}
		
		if(IsWayPointGotten())
		{
			OnWayPointGotten();
		}
	}
}
