using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MovableObject))]
public class MoveAroundSimulator : MonoBehaviour 
{
	public GameObject movingAroundGameObject;
	
	private MovableObject movableObject;
	
	// Use this for initialization
	void Start() 
	{
		movableObject = GetComponent<MovableObject>();
	}
	
	void Move()
	{
		Vector3 fromMovingAroundPointToSelfDirection = transform.position - movingAroundGameObject.transform.position;
		Vector3 movingDirection = Vector3.Cross(Vector3.up, fromMovingAroundPointToSelfDirection);
		
		movableObject.Move(movingDirection);
	}
	
	// Update is called once per frame
	void Update() 
	{
		Move();
	}

}