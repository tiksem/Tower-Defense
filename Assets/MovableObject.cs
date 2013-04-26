using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterMotor))]
[RequireComponent (typeof (Animation))]
public class MovableObject : MonoBehaviour 
{
	private CharacterMotor motor;
	private CharacterController characterController;
	private Animation animation;
	
	// Use this for initialization
	void Start() 
	{
		motor = GetComponent<CharacterMotor>();
		animation = GetComponent<Animation>();
		characterController = GetComponent<CharacterController>();
	}
	
	public void Move(Vector3 movingDirection)
	{
		movingDirection.Normalize();
		
		motor.inputMoveDirection = movingDirection;
		transform.rotation = Quaternion.LookRotation(movingDirection);
		ApplyRunAnimation();
	}
	
	private void ApplyRunAnimation()
	{
		animation.Play("run");
	}
}