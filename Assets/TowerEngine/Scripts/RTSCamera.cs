using UnityEngine;
using System.Collections;

[RequireComponent( typeof(Camera) )]
class RTSCamera : MonoBehaviour
{
 	public float maxX = 100.0f;
	public float maxY = 50.0f;
	public float maxZ = 100.0f;
	public float minX = 0.0f;
	public float minY = 8.0f;
	public float minZ = 0.0f;
	
	public float zoomSpeed = 1.0f;
	public float scrollSpeed = 1.0f;
	
	private void ValidateCameraPosition()
	{
		Vector3 cameraPosition = transform.position;
		
		if(cameraPosition.x > maxX)
		{
			cameraPosition.x = maxX;
		}
		else if(cameraPosition.x < minX)
		{
			cameraPosition.x = minX;
		}
		
		if(cameraPosition.y > maxY)
		{
			cameraPosition.y = maxY;
		}
		else if(cameraPosition.y < minY)
		{
			cameraPosition.y = minY;
		}
		
		if(cameraPosition.z > maxZ)
		{
			cameraPosition.z = maxZ;
		}
		else if(cameraPosition.z < minZ)
		{
			cameraPosition.z = minZ;
		}
		
		transform.position = cameraPosition;
	}
	
	void OnVaildate()
	{
		ValidateCameraPosition();
	}
	
	void Start()
	{
		ValidateCameraPosition();
	}
}
