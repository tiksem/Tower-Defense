using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[System.Serializable]
	public class BarWithCircleButtons
	{
		public float iconWidth;
		public float iconHeight;
		public float firstIconLeft;
		public float iconBottom;
		public float distanceBetweenIcons;
		
		private Vector2[] buttons;
		private float buttonRadius;
		
		public void Update(int buttonsCount, GUITexture guiTexture)
		{
			buttons = new Vector2[buttonsCount];
		
			float x = firstIconLeft;
			float y = 1.0f - iconBottom - iconHeight / 2;
			float iconY = y * guiTexture.pixelInset.height;
			float textureWidth = guiTexture.pixelInset.width;
			buttonRadius = iconWidth / 2 * textureWidth;
		
			for(int i = 0; i < buttonsCount; i++)
			{
				float iconX = x + iconWidth / 2;
			
				x += iconWidth;
				x += distanceBetweenIcons;
			
				iconX *= textureWidth;
			
				buttons[i] = new Vector2(iconX, iconY);
			}
		}
		
		public int GetClickedButtonIndex()
		{
			Vector2 mouse = Input.mousePosition;
			for(int i = 0; i < buttons.Length; i++)
			{
				if(Utilities.IsPointInsideCircle(mouse, buttons[i], buttonRadius))
				{
					return i;
				}
			}
		
			return -1;
		}
	}
}

