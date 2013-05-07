using UnityEngine;
using System.Collections;

public abstract class GuiEventsHandler : MonoBehaviour
{
	private GUIElement guiElement;  
	private bool lastMouseDownOn = false;
	
	virtual protected void OnMouseDown()
	{
		
	}
	
	virtual protected void OnMouseUp()
	{
		
	}
	
	virtual protected void OnClick()
	{
		
	}
	
	private bool IsMouseOn()
	{
		return guiElement.HitTest(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
	}
	
	private void UpdateMouseEvents()
	{
		bool isMouseOn = IsMouseOn();
		
		if(Input.GetMouseButtonDown(0))
		{
			lastMouseDownOn = isMouseOn;
			
			if(isMouseOn)
			{
				OnMouseDown();
			}
		}
		
		if(isMouseOn && Input.GetMouseButtonUp(0))
		{
			OnMouseUp();
			
			if(lastMouseDownOn)
			{
				OnClick();
			}
		}
	}
	
	// Use this for initialization
	void Start()
	{
		guiElement = GetComponent<GUIElement>();
		if(guiElement == null)
		{
			throw new System.ArgumentException("GuiEventsHandler should be attached to GUIElement");
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		UpdateMouseEvents();
	}
}
