using UnityEngine;
using System.Collections;
using System;

public class GuiEventsHandler
{
	private GUIElement guiElement;  
	private bool lastMouseDownOn = false;
	
	public enum State
	{
		NONE,
		CLICK,
		DOWN,
		UP,
	}
	
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
	
	private State UpdateMouseEvents()
	{
		bool isMouseOn = IsMouseOn();
		State state = State.NONE;
		
		if(Input.GetMouseButtonDown(0))
		{
			lastMouseDownOn = isMouseOn;
			
			if(isMouseOn)
			{
				if(onMouseDown != null)
				{
					onMouseDown();
				}
				
				return State.DOWN;
			}
		}
		
		if(isMouseOn && Input.GetMouseButtonUp(0))
		{
			if(onMouseUp != null)
			{
				onMouseUp();
			}
			
			state = State.UP;
			
			if(lastMouseDownOn)
			{
				state = State.CLICK;
				
				if(onMouseClick != null)
				{
					onMouseClick();
				}
			}
		}
		
		return state;
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
	public State Update()
	{
		return UpdateMouseEvents();
	}
}
