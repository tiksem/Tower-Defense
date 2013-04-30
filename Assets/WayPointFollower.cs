using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (MovableObject))]
public class WayPointFollower : MonoBehaviour 
{
	public float timeBeforeMoving = 0.0f;
	
	private GameObject[] wayPoints;
	private int moveToWayPointIndex;
	private MovableObject movableObjectComponent;
	
	private string wayPointTag;
	
	public void SetWayPointTag(string wayPointTag)
	{
		this.wayPointTag = wayPointTag;
		wayPoints = GameObject.FindGameObjectsWithTag(wayPointTag);
		InitWayPoints();
	}
	
	private int WayPointsComparator(GameObject a, GameObject b)
	{
		int aId = int.Parse(a.name);
		int bId = int.Parse(b.name);
		return aId.CompareTo(bId);
	}
	
	private void SortWayPoints()
	{
		System.Array.Sort(wayPoints, WayPointsComparator);
	}
	
	private GameObject GetCurrentWayPoint()
	{
		if(moveToWayPointIndex < wayPoints.Length)
		{
			return wayPoints[moveToWayPointIndex];
		}
		else
		{
			return null;
		}
	}
	
	private Vector3 GetCurrentWayPointPosition()
	{
		GameObject wayPoint = GetCurrentWayPoint();
		return wayPoint.transform.position;
	}
	
	private void MoveToNextWayPoint()
	{
		Vector3 wayPointPosition = GetCurrentWayPointPosition();
		movableObjectComponent.MoveToPoint(wayPointPosition);
	}
	
	private void MoveNext()
	{
		if(HasNextWayPoint())
		{
			MoveToNextWayPoint();
		}
		else
		{
			OnFinish();
		}
	}
	
	private void OnFinish()
	{
		
	}
	
	private bool HasNextWayPoint()
	{
		return moveToWayPointIndex < wayPoints.Length;
	}
	
	private void OnWayPointGotten()
	{
		moveToWayPointIndex++;
		MoveNext();
	}
	
	private void Move()
	{
		if(!movableObjectComponent.IsMoving())
		{
			MoveNext();
		}
	}
	
	private IEnumerator StartMovingWithTimerCoroutine()
	{
		yield return new WaitForSeconds(timeBeforeMoving);
		Move();
	}
	
	private void StartMovingWithTimer()
	{
		StartCoroutine(StartMovingWithTimerCoroutine());
	}
	
	void InitWayPoints()
	{
		wayPoints = GameObject.FindGameObjectsWithTag(wayPointTag);
		SortWayPoints();
		StartMovingWithTimer();
	}
	
	// Use this for initialization
	void Start() 
	{
		moveToWayPointIndex = 0;
		movableObjectComponent = GetComponent<MovableObject>();
		movableObjectComponent.onStop = OnWayPointGotten;
	}
	
	// Update is called once per frame
	void Update() 
	{
	
	}
}
