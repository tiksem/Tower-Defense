using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class MouseClickHandler
	{
#if UNITY_EDITOR
		private static float MAX_CLICK_OFFSET = 0.1f;
#else
		private static float MAX_CLICK_OFFSET = 20.0f;
#endif
		private Vector2 onMouseDownPosition;
		private Vector2 onMouseUpPosition;
		
		public MouseClickHandler()
		{
			isClickAccepted = DefaultIsClickAccepted;
		}
		
		public static bool DefaultIsClickAccepted(Vector2 onMouseDownPosition, Vector2 onMouseUpPosition)
		{
			return Vector3.Distance(onMouseDownPosition, onMouseUpPosition) <= MAX_CLICK_OFFSET;
		}
		
		public Func<Vector2, Vector2, bool> isClickAccepted;
		public Func<Void> onClick;
		
		private bool IsClickAccepted()
		{
			return isClickAccepted(onMouseDownPosition, onMouseUpPosition);
		}
		
		private bool CheckClick()
		{
			if(IsClickAccepted())
			{
				if(onClick != null)
				{
					onClick();
				}
				
				return true;
			}
			
			return false;
		}
		
		public bool Update()
		{
			bool eventFired = false;
			
			if(Input.GetMouseButtonDown(0))
			{
        		onMouseDownPosition = Input.mousePosition;
    		}
		
    		if(Input.GetMouseButtonUp(0))
			{
        		onMouseUpPosition = Input.mousePosition;
				eventFired = CheckClick();
    		}
			
			return eventFired;
		}
	}
}

