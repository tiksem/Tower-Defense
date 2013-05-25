using UnityEngine;
using System.Collections;
using System;

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
	private bool isLaserOnTarget = false;
	private bool effectsAttached = false;
	
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
		isLaserOnTarget = true;
		effectsAttached = false;
		yield return new WaitForSeconds(delayBeforeDestroy);
		base.DestroyGameObject();
		isLaserOnTarget = false;
	}
	
	protected override void AttachEffectsToTargets(Target[] targets)
	{
		if(!effectsAttached)
		{
			base.AttachEffectsToTargets(targets);
			effectsAttached = true;
		}
	}
	
	protected override void DamageTarget(Target target, int damage)
	{
		base.DamageTarget(target, Mathf.RoundToInt(damage * Time.deltaTime));
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
