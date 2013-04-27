using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (CharacterMotor))]
[RequireComponent (typeof (Animation))]
public class MovableObject : MonoBehaviour 
{
	public float rotationSpeed = 1.0f;
	
	private CharacterMotor motor;
	private CharacterController characterController;
	private Animation animation;
	private Vector3 movingDirection = Vector3.zero;
	private bool smoothRotationActivated = false;
	private float shouldBeStopedAfterSqrDistance = float.PositiveInfinity;
	private Vector3 positionBeforeStartingMoving;
	
	public Func<System.Void> onStop;
	
	private void ResetPositionBeforeStartingMoving()
	{
		positionBeforeStartingMoving = transform.position;
	}
	
	// Use this for initialization
	void Start() 
	{
		motor = GetComponent<CharacterMotor>();
		animation = GetComponent<Animation>();
		characterController = GetComponent<CharacterController>();
		ResetPositionBeforeStartingMoving();
	}
	
	private void ApplyRudeRotation()
	{
		transform.rotation = Quaternion.LookRotation(movingDirection);
	}
	
	private void ApplySmoothRotation()
	{
		float rotationPortion = Time.deltaTime * rotationSpeed;
		Quaternion lookAt = Quaternion.LookRotation(movingDirection);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, rotationPortion);
	}
	
	public void MoveWithSmoothyRotation(Vector3 movingDirection)
	{
		Move(movingDirection, true);
	}
	
	private void StartMoving(Vector3 movingDirection, bool rotateSmoothy)
	{
		if(movingDirection == Vector3.zero)
		{
			throw new System.ArgumentException("movingDirection = Vector3.zero");
		}
		
		ResetPositionBeforeStartingMoving();
		
		movingDirection.Normalize();
		
		this.movingDirection = motor.inputMoveDirection = movingDirection;
		
		if(!rotateSmoothy)
		{
			ApplyRudeRotation();
			smoothRotationActivated = false;
		}
		else
		{
			smoothRotationActivated = true;
		}
		
		ApplyRunAnimation();
	}
	
	public void Move(Vector3 movingDirection, bool rotateSmoothy = false)
	{
		StartMoving(movingDirection, rotateSmoothy);
		shouldBeStopedAfterSqrDistance = float.PositiveInfinity;
	}
	
	private Func<Void> drawLineDebug = null;
	
	public void MoveToPoint(Vector3 moveToPoint, bool rotateSmoothy = false)
	{
		Vector3 movingDirection = moveToPoint - transform.position;
		
		drawLineDebug = () => Debug.DrawLine(transform.position, moveToPoint);
		
		StartMoving(movingDirection, rotateSmoothy);
		shouldBeStopedAfterSqrDistance = movingDirection.sqrMagnitude;
	}
	
	public void MoveToPointWithSmoothyRotation(Vector3 movingDirection)
	{
		MoveToPoint(movingDirection, true);
	}
	
	private void OnStop()
	{
		if(onStop != null)
		{
			onStop();
		}
	}
	
	public void Stop()
	{
		movingDirection = motor.inputMoveDirection = Vector3.zero;
		ApplyStopAnimation();
		OnStop();
	}
	
	public bool IsMoving()
	{
		return motor.inputMoveDirection != Vector3.zero;
	}
	
	private Vector3 GetMovingWayVector()
	{
		return transform.position - positionBeforeStartingMoving;
	}
	
	private bool ShoudBeStopped()
	{
		if(float.IsPositiveInfinity(shouldBeStopedAfterSqrDistance))
		{
			return false;
		}
		
		Vector3 way = GetMovingWayVector();
		return way.sqrMagnitude >= shouldBeStopedAfterSqrDistance;
	}
	
	void Update()
	{
		if(IsMoving())
		{
			if(smoothRotationActivated)
			{
				ApplySmoothRotation();
			}
			
			if(ShoudBeStopped())
			{
				Stop();
			}
		}
		
		if(drawLineDebug != null)
		{
			drawLineDebug();
		}
	}
	
	private void ApplyStopAnimation()
	{
		animation.Stop("run");
		animation.Play("idle");
	}
	
	private void ApplyRunAnimation()
	{
		animation.Stop("idle");
		animation.Play("run");
	}
}