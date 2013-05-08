using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class MouseClickHandler
	{	
		private Vector3 onMouseDownPosition;
		private Vector3 onMouseUpPosition;
		
		public MouseClickHandler()
		{
			isClickAccepted = DefaultIsClickAccepted;
		}
		
		public static bool DefaultIsClickAccepted(Vector3 onMouseDownPosition, Vector3 onMouseUpPosition)
		{
			return onMouseDownPosition == onMouseUpPosition;
		}
		
		public Func<Vector3, Vector3, bool> isClickAccepted;
		public Func<Void> onClick;
		
		private bool IsClickAccepted()
		{
			return isClickAccepted(onMouseDownPosition, onMouseUpPosition);
		}
		
		private void CheckClick()
		{
			if(IsClickAccepted())
			{
				onClick(); 
			}
		}
		
		public void Update()
		{
			if(Input.GetMouseButtonDown(0))
			{
        		onMouseDownPosition = Input.mousePosition;
    		}
		
    		if(Input.GetMouseButtonUp(0))
			{
        		onMouseUpPosition = Input.mousePosition;
				CheckClick();
    		}
		}
	}
}

