using UnityEngine;
using AssemblyCSharp;

public class RTSCamera : MonoBehaviour 
{
	[System.Serializable]
	public class TouchSettings
	{
    	public float minTouchZoomSpeed = 5.0F;
    	public float zoomTouchDeltaOffset = 5.0F;
		public float moveSpeed = 100.0f;
	}
	
	[System.Serializable]
	public class MouseSettings
	{
    	public float zoomSpeed = 5.0f;
	}
	
    public float zoomSpeed = 4;
	public float dragSpeed = 0.7f;
	public float maxFieldOfViewSize = 90;
	public float minFieldOfViewSize = 15;
	public float mapStartX = 0.0f;
	public float mapStartZ = 0.0f;
	public float mapWidth = 100.0f;
	public float mapHeight = 100.0f;
 
	public TouchSettings touchSettings;
	public MouseSettings mouseSettings;
	
    private Camera selectedCamera;
	
	Vector3 lastMouseMovingPosition = Vector3.zero;
	Vector3 currentMouseMovingPosition = Vector3.zero;
	
    // Use this for initialization
    void Start () 
    {
 		selectedCamera = GetComponent<Camera>();
		ValidateCameraPosition();
    }
	
	private static float GetTouchSpeed(Touch touch)
	{
		return touch.deltaPosition.magnitude / touch.deltaTime;
	}
	
	private bool IsTouchSpeedAccepted(float touchSpeed1, float touchSpeed2)
	{
		return touchSpeed1 > touchSettings.minTouchZoomSpeed && touchSpeed2 > touchSettings.minTouchZoomSpeed;
	}
	
	private void ChangeCameraFieldOfView(float offset)
	{
		if(offset == 0.0f)
		{
			return;
		}
		
		selectedCamera.fieldOfView = Mathf.Clamp(selectedCamera.fieldOfView + offset, minFieldOfViewSize, maxFieldOfViewSize);
	}
	
	private void MoveCameraToScreenPoint(Vector2 sreenPoint)
	{
		Vector3 cameraPosition = CameraUtilities.ScreenToWorldPoint(selectedCamera, sreenPoint);
		cameraPosition.y = selectedCamera.transform.position.y;
		selectedCamera.transform.position = cameraPosition;
	}
	
	private void ValidateCameraPosition()
	{
		ScreenDiagonal screenDiagonal = CameraUtilities.GetCameraWorldDiagonalPoints(selectedCamera);
		Vector3 leftBottom = screenDiagonal.leftBottom;
		Vector3 rightTop = screenDiagonal.rightTop;
		
		bool validateMinXSuccess = leftBottom.x >= mapStartX;
		bool validateMinZSuccess = leftBottom.z >= mapStartZ;
		bool validateMaxZSuccess = rightTop.z <= mapStartZ + mapHeight;
		bool validateMaxXSuccess = rightTop.x <= mapStartX + mapWidth;
		
		if((!validateMinXSuccess && !validateMaxXSuccess) || (!validateMinZSuccess && !validateMaxZSuccess))
		{
			throw new System.ArgumentException("Camera position validation failed, check your camera settings");
		}
		
		Vector3 cameraPosition = selectedCamera.transform.position;
		
		if(!validateMinXSuccess)
		{
			float xDif = leftBottom.x - mapStartX;
			cameraPosition.x -= xDif;
		}
			
		if(!validateMinZSuccess)
		{
			float zDif = leftBottom.z - mapStartZ;
			cameraPosition.z -= zDif;
		}
		
		if(!validateMaxXSuccess)
		{
			float xDif = rightTop.x - mapStartX - mapWidth;
			cameraPosition.x -= xDif;
		}
			
		if(!validateMaxZSuccess)
		{
			float zDif = rightTop.z - mapStartZ - mapHeight;
			cameraPosition.z -= zDif;
		}
		
		selectedCamera.transform.position = cameraPosition;
	}
	
	private void SetCameraPosition(Vector2 screenPoint, float zoomSpeed)
	{
		ChangeCameraFieldOfView(zoomSpeed);
		
		if(zoomSpeed < 0.0f)
		{
			MoveCameraToScreenPoint(screenPoint);
		}
		
		ValidateCameraPosition();
	}
	
	private void LeftMouseDrag()
	{
    	// From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
    	// with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
    	currentMouseMovingPosition.z = lastMouseMovingPosition.z = transform.position.y;

    	// Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
    	// anyways.  
    	Vector3 direction = Camera.main.ScreenToWorldPoint(currentMouseMovingPosition) - Camera.main.ScreenToWorldPoint(lastMouseMovingPosition);
		direction.y = 0;

    	// Invert direction to that terrain appears to move with the mouse.
    	direction = direction * -1 * dragSpeed;
    	transform.position += direction;
		
		lastMouseMovingPosition = currentMouseMovingPosition;
		
		ValidateCameraPosition();
	}
	
	private void UpdateMouseMoving()
	{
		if(Input.GetMouseButtonDown(0))
		{
        	lastMouseMovingPosition = Input.mousePosition;
    	}
		
    	if(Input.GetMouseButton(0))
		{
        	currentMouseMovingPosition = Input.mousePosition;
        	LeftMouseDrag();        
    	}
	}
	
	private void UpdateTouchMoving(Touch touch)
	{
		
	}
	
	private void UpdateTouchZoom(Touch touch1, Touch touch2)
	{
		Vector2 currentTouchDistance = touch1.position - touch2.position;
		Vector2 touchDelta1 = touch1.position - touch1.deltaPosition;
		Vector2 touchDelta2 = touch2.position - touch2.deltaPosition;
		Vector2 prevTouchDistance = touchDelta1 - touchDelta2;
		
		float touchDelta = currentTouchDistance.magnitude - prevTouchDistance.magnitude + touchSettings.zoomTouchDeltaOffset;
		float touchSpeed1 = GetTouchSpeed(touch1);
		float touchSpeed2 = GetTouchSpeed(touch2);
		
		if(IsTouchSpeedAccepted(touchSpeed1, touchSpeed2))
		{
			if(touchDelta <= 1)
			{
				SetCameraPosition(prevTouchDistance, zoomSpeed);
			}
			else
			{
				SetCameraPosition(prevTouchDistance, -zoomSpeed);
			}
		}
	}
	
	private void UpdateTouchEvents()
	{
		if(Input.touchCount == 2)
		{
			Touch touch1 = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);
			
			if(touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
			{
				UpdateTouchZoom(touch1, touch2);
			}
		}
		else if(Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			
			if(touch.phase == TouchPhase.Moved)
			{
				UpdateTouchMoving(touch);
			}
		}
	}
	
	private void UpdateMouseEvents()
	{
		UpdateMouseMoving();
	}
	
	private void UpdateMouseWheelEvents()
	{
		float mouseWheelDelta = Input.GetAxis("Mouse ScrollWheel");
		if(mouseWheelDelta == 0.0f)
		{
			return;
		}
		
		Vector3 cursorPosition = Input.mousePosition;
		SetCameraPosition(cursorPosition, -zoomSpeed * mouseWheelDelta * mouseSettings.zoomSpeed);
	}
	
    // Update is called once per frame
    void Update () 
    {
 		UpdateTouchEvents();
		UpdateMouseWheelEvents();
		UpdateMouseEvents();
    }
}