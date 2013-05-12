using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class GuiEventsHandlerCombiner
	{
		private GuiEventsHandler[] handlers;
		
		public void CreateHandlersFor(GUIElement[] guiElements)
		{
			int handlersCount = guiElements.Length;
			handlers = new GuiEventsHandler[handlersCount];
			
			for(int i = 0; i < handlers.Length; i++)
			{
				GUIElement guiElement = guiElements[i];
				if(guiElement == null)
				{
					continue;
				}
				
				GameObject gameObject = guiElement.gameObject;
				GuiEventsHandler guiEventsHandler = new GuiEventsHandler(gameObject);
				handlers[i] = guiEventsHandler;
			}
		}
		
		public GuiEventsHandlerCombiner ()
		{
			
		}
		
		public GuiEventsHandlerCombiner(GUIElement[] guiElements)
		{
			CreateHandlersFor(guiElements);
		}
		
		public int GetClickedGUIElementIndex()
		{
			int handlersCount = handlers.Length;
			for(int i = 0; i < handlersCount; i++)
			{
				GuiEventsHandler guiEventsHandler = handlers[i];
				if(guiEventsHandler == null)
				{
					continue;
				}
				
				if(guiEventsHandler.Update() == GuiEventsHandler.State.CLICK)
				{
					return i;
				}
			}
			
			return -1;
		}
	}
}

