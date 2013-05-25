using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LaserBullet : Bullet
{
	public float velocity = 1.0f;
	public float delayBeforeDestroy = 1.0f;
	
	private LineRenderer lineRenderer;
	private bool isTargetHit = false;
	Vector3 startPosition;
	private Vector3 laserEndPosition;
	private float startTime;
	
	private float GetTimeElapsed()
	{
		return Time.time - startTime;
	}
	
	private void UpdateLaserEndPosition()
	{
		Vector3 targetPosition = GetTargetPosition();
		Vector3 direction = targetPosition - startPosition;
		float maxDistance = direction.magnitude;
		Vector3 normilizedDirection = direction / maxDistance;
		
		float timeElapsed = GetTimeElapsed();
		float movingDistance = velocity * timeElapsed;
		
		if(movingDistance < maxDistance)
		{
			 laserEndPosition = startPosition + normilizedDirection * movingDistance;
		}
		else
		{
			laserEndPosition = targetPosition;
			isTargetHit = true;
		} 
		
		lineRenderer.SetPosition(1, laserEndPosition);
	}
	
	// Use this for initialization
	void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		startPosition = transform.position;
		laserEndPosition = startPosition;
		lineRenderer.SetPosition(0, laserEndPosition);
		startTime = Time.time;
		
		UpdateLaserEndPosition();
	}
	
	protected override void MoveToTarget()
	{
		UpdateLaserEndPosition();
	}
	
	private IEnumerator DestroyAction()
	{
		yield return new WaitForSeconds(delayBeforeDestroy);
		base.DestroyGameObject();
	}
	
	protected override void DestroyGameObject()
	{
		StartCoroutine("DestroyAction");
	}
	
	protected override TargetHit CheckTargetHit()
	{
		if(target == null)
		{
			return new TargetHit();
		}
		
		if(isTargetHit)
		{
			return CreateTargetHitForCurrentTarget();
		}
		else
		{
			return null;
		}
	}
}
