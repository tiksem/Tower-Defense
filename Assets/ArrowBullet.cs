using UnityEngine;
using System.Collections;

public class ArrowBullet : Bullet 
{
	public float velocity = 1.0f;
	
	private Vector3 firstPosition;
	
	private void LookAtTarget()
	{
		if(target != null)
		{
			transform.LookAt(target.transform);
		}
	}
	
	// Use this for initialization
	void Start() 
	{
		firstPosition = transform.position;
		LookAtTarget();
	}
	
	protected override void MoveToTarget()
	{
		LookAtTarget();
		Vector3 translation = transform.TransformDirection(Vector3.forward);
		translation = translation.normalized * velocity * Time.deltaTime;
		transform.position += translation;
	}
	
	protected override bool IsTargetHit()
	{
		Vector3 targetPosition = GetTargetPosition();
		Vector3 firstPositionToTarget = targetPosition - firstPosition;
		Vector3 firstPositionToCurrent = transform.position - firstPosition;
		return firstPositionToCurrent.sqrMagnitude >= firstPositionToTarget.sqrMagnitude;
	}
}
