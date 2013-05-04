using UnityEngine;
using AssemblyCSharp;

public class CameraZoomPinch : MonoBehaviour 
{
	[System.Serializable]
	public class TouchSettings
	{
    	public float minTouchZoomSpeed = 5.0F;
    	public float zoomTouchDeltaOffset = 5.0F;
		public float moveSpeed = 100.0f;
	}
	
    public float zoomSpeed = 4;
	public float maxFieldOfViewSize = 90;
	public float minFieldOfViewSize = 15;
	public float mapStartX = 0.0f;
	public float mapStartZ = 0.0f;
	public float mapWidth = 100.0f;
	public float mapHeight = 100.0f;
 
	public TouchSettings touchSettings;
	
    private Camera selectedCamera;
	
	private float lastMouseX = -1.0f;
	private float lastMouseY = -1.0f;
	
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
	
	private bool ValidateCameraMinXPosition(float leftX)
	{
		if(leftX >= mapStartX)
		{
			return true;
		}
		
		Vector3 cameraPosition = selectedCamera.transform.position;
		cameraPosition.x = mapStartX;
		selectedCamera.transform.position = cameraPosition;
		
		return false;
	}
	
	private bool ValidateCameraMinZPosition(float bottomZ)
	{
		if(bottomZ >= mapStartZ)
		{
			return true;
		}
		
		Vector3 cameraPosition = selectedCamera.transform.position;
		cameraPosition.z = mapStartZ;
		selectedCamera.transform.position = cameraPosition;
		
		return false;
	}
	
	private bool ValidateCameraMaxZPosition(float topZ)
	{
		var maxZ = mapStartZ + mapHeight;
		if(topZ <= maxZ)
		{
			return true;
		}
		
		Vector3 cameraPosition = selectedCamera.transform.position;
		cameraPosition.z = maxZ;
		selectedCamera.transform.position = cameraPosition;
		
		return false;
	}
	
	private bool ValidateCameraMaxXPosition(float rightX)
	{
		var maxX = mapStartX + mapWidth;
		if(rightX <= maxX)
		{
			return true;
		}
		
		Vector3 cameraPosition = selectedCamera.transform.position;
		cameraPosition.x = maxX;
		selectedCamera.transform.position = cameraPosition;
		
		return false;
	}
	
	private void ValidateCameraPosition()
	{
		ScreenDiagonal screenDiagonal = CameraUtilities.GetCameraWorldDiagonalPoints(selectedCamera);
		Vector3 leftBottom = screenDiagonal.leftBottom;
		Vector3 rightTop = screenDiagonal.rightTop;
		
		bool validateMinXSuccess = ValidateCameraMinXPosition(leftBottom.x);
		bool validateMinZSuccess = ValidateCameraMinZPosition(leftBottom.z);
		bool validateMaxZSuccess = ValidateCameraMaxZPosition(rightTop.z);
		bool validateMaxXSuccess = ValidateCameraMaxXPosition(rightTop.x);
		
		if((!validateMinXSuccess && !validateMaxXSuccess) || (!validateMinZSuccess && !validateMaxZSuccess))
		{
			throw new System.ArgumentException("Camera position validation failed, check your camera settings");
		}
	}
	
	private void SetCameraPosition(Vector2 screenPoint, float zoomSpeed)
	{
		ChangeCameraFieldOfView(zoomSpeed);
		MoveCameraToScreenPoint(screenPoint);
		ValidateCameraPosition();
	}
	
	private void UpdateMouseMoving()
	{
		if(lastMouseX < 0.0f)
		{
			lastMouseX = Input.mousePosition.x;
			lastMouseY = Input.mousePosition.y;
			return;
		}
		
		float mouseX = Input.mousePosition.x - lastMouseX;
        float mouseY = Input.mousePosition.y - lastMouseY;
		Vector3 newCameraPosition = CameraUtilities.ScreenToWorldPoint(selectedCamera, new Vector2(mouseX, mouseY));
		newCameraPosition.y = selectedCamera.transform.position.y;
      	selectedCamera.transform.position = Vector3.Lerp(selectedCamera.transform.position, newCameraPosition, Time.deltaTime);
		
		lastMouseX = Input.mousePosition.x;
		lastMouseY = Input.mousePosition.y;
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
		// only for pc
		if(Input.touchCount != 0)
		{
			return;
		}
		
		if(Input.GetMouseButton(0))
		{
			UpdateMouseMoving();
		}
		else
		{
			lastMouseX = -1.0f;
			lastMouseY = -1.0f;
		}
	}
	
	private void UpdateMouseWheelEvents()
	{
		float mouseWheelDelta = Input.GetAxis("Mouse ScrollWheel");
		if(mouseWheelDelta == 0.0f)
		{
			return;
		}
		
		Vector3 cursorPosition = Input.mousePosition;
		SetCameraPosition(cursorPosition, -zoomSpeed * mouseWheelDelta);
	}
	
    // Update is called once per frame
    void Update () 
    {
 		UpdateTouchEvents();
		UpdateMouseWheelEvents();
		UpdateMouseEvents();
    }
}