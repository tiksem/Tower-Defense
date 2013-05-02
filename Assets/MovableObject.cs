using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (RigidBodyCharacterController))]
[RequireComponent (typeof (Animation))]
public class MovableObject : MonoBehaviour 
{
	[HideInInspector]
	public Func<System.Void> onStop;
	
	public float pointSqrApproximationRadius = 50.0f;
	public float stopSqrApproximationRadius = 50.0f;
	public float rotationTime = 1.0f;
	
	private RigidBodyCharacterController characterController;
	private Vector3 moveToPoint;
	private Quaternion rotateTo;
	private Vector3 positionBeforeMoveToPointInvoked;
	private float sqrMagnitudeToPoint;
	
	private void ChangeMovingDirection(Vector3 direction)
	{
		characterController.movingDirection = direction;
		rotateTo = Quaternion.LookRotation(direction);
	}
	
	public void Move(Vector3 direction)
	{
		moveToPoint = Vector3.zero;
		PlayRunAnimation();
		ChangeMovingDirection(direction);
	}
	
	private Vector3 GetMoveToPointDirection()
	{
		return moveToPoint - transform.position;
	}
	
	private float GetMoveToPointPassedSqrMagnitude()
	{
		Vector3 passed = transform.position - positionBeforeMoveToPointInvoked;
		return passed.sqrMagnitude;
	}
	
	public void MoveToPoint(Vector3 point)
	{
		moveToPoint = point;
		positionBeforeMoveToPointInvoked = transform.position;
		Vector3 moveToPointDirection = GetMoveToPointDirection();
		sqrMagnitudeToPoint = moveToPointDirection.sqrMagnitude;
	}
	
	private float GetsqrMagnitudeDifBetweenMoveToPointAndPosition()
	{
		return Math.Abs(transform.position.sqrMagnitude - moveToPoint.sqrMagnitude);
	}
	
	private bool ShouldBeStopped()
	{
		float passedSqrMagnitude = GetMoveToPointPassedSqrMagnitude();
		return passedSqrMagnitude >= sqrMagnitudeToPoint - stopSqrApproximationRadius;
	}
	
	private void UpdateMoveToPointState()
	{
		if(moveToPoint == Vector3.zero)
		{
			return;
		}
		
		if(ShouldBeStopped())
		{
			Stop();
		}
		else
		{
			if(GetsqrMagnitudeDifBetweenMoveToPointAndPosition() <= pointSqrApproximationRadius)
			{
				return;
			}
			
			Vector3 direction = GetMoveToPointDirection();
			ChangeMovingDirection(direction.normalized);
		}
	}
	
	private void PlayStopAnimation()
	{
		animation.Play("idle");
	}
	
	private void PlayRunAnimation()
	{
		animation.Play("run");
	}
	
	private void OnStop()
	{
		if(onStop != null)
		{
			onStop();
		}
		
		PlayStopAnimation();
	}
	
	public void Stop()
	{
		characterController.movingDirection = Vector3.zero;
		moveToPoint = Vector3.zero;
		rotateTo = Quaternion.identity;
		onStop();
	}
	
	public bool IsMoving()
	{
		return characterController.movingDirection != Vector3.zero;
	}
	
	void Start()
	{
		characterController = GetComponent<RigidBodyCharacterController>();
	}
	
	private void UpdateRotationState()
	{
		if(rotateTo != Quaternion.identity)
		{
			if(rotateTo != transform.rotation)
			{
				float rotationDeltaSize = Time.deltaTime / rotationTime;
        		rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, rotateTo, rotationDeltaSize);
			}
		}
	}
	
	public void Rotate(Quaternion rotation)
	{
		rotateTo = rotation;
	}
	
	void FixedUpdate()
	{
		UpdateRotationState();
	}
	
	void Update()
	{
		UpdateMoveToPointState();
	}
}