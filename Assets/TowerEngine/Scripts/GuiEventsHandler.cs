using UnityEngine;
using System.Collections;
using System;

public class GuiEventsHandler
{
	private GUIElement guiElement;  
	private bool lastMouseDownOn = false;

	public Func<System.Void> onMouseDown;
	public Func<System.Void> onMouseUp;
	public Func<System.Void> onMouseClick;
	
	virtual protected void OnClick()
	{
		
	}
	
	private bool IsMouseOn()
	{
		bool isActive = guiElement.enabled && guiElement.gameObject.activeSelf;
		return isActive && guiElement.HitTest(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
	}
	
	private bool UpdateMouseEvents()
	{
		bool isMouseOn = IsMouseOn();
		bool eventFired = false;
		
		if(Input.GetMouseButtonDown(0))
		{
			lastMouseDownOn = isMouseOn;
			
			if(isMouseOn)
			{
				if(onMouseDown != null)
				{
					onMouseDown();
				}
				
				eventFired = true;
			}
		}
		
		if(isMouseOn && Input.GetMouseButtonUp(0))
		{
			if(onMouseUp != null)
			{
				onMouseUp();
			}
			
			eventFired = true;
			
			if(lastMouseDownOn && onMouseClick != null)
			{
				onMouseClick();
			}
		}
		
		return eventFired;
	}

	public GuiEventsHandler(GameObject guiObject)
	{
		guiElement = guiObject.GetComponent<GUIElement>();
		if(guiElement == null)
		{
			throw new System.ArgumentException("GuiEventsHandler should be attached to GUIElement");
		}
	}
	
	// Update is called once per frame
	public bool Update()
	{
		return UpdateMouseEvents();
	}
}
