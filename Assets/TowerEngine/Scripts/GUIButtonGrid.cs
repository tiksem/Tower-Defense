using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[System.Serializable]
	public class GUIButtonGrid
	{
		public float buttonSize = 0.1f;
		public float distanceBetweenButtons = 0.05f;
		public float buttonsLeft = 0.1f;
		public float buttonsTop = 0.05f;
		public int buttonsOnLine = 2;
		
		public Func<int,Void> onClick;
		public Func<int, Rect, Void> additionalDataDrawer;
		
		private Texture[] buttons;
		
		public void SetButtons(Texture[] buttons)
		{
			this.buttons = buttons;
		}
		
		public void SetButtons<T>(T[] buttonsSource, Func<T, Texture> transformer)
		{
			int buttonsCount = buttonsSource.Length;
			buttons = new Texture[buttonsCount];
			for(int i = 0; i < buttonsCount; i++)
			{
				buttons[i] = transformer(buttonsSource[i]);	
			}
		}
		
		public void SetButton(int index, Texture texture)
		{
			buttons[index] = texture;
		}
		
		Rect GetScrollViewRect()
		{
			float width = 1.0f - buttonsLeft;
			float height = 1.0f - buttonsTop;
			return GUIUtilities.ScreenToGUIRect(buttonsLeft, buttonsTop, width, height);
		}
		
		private void OnClick(int index)
		{
			if(onClick != null)
			{
				onClick(index);
			}
		}
		
		public void Draw()
		{
			float x = buttonsLeft;
			float y = buttonsTop;
			float buttonHeight = GUIUtilities.GetHeightFromWidthForSquareButton(buttonSize);
			float xOffset = distanceBetweenButtons + buttonSize;
			float yOffset = GUIUtilities.GetHeightFromWidthForSquareButton(distanceBetweenButtons) +
				buttonHeight;
			
			Rect scrollViewRect = GetScrollViewRect();
			Vector2 scroll = new Vector2(0.0f, 1.0f);
			GUI.BeginScrollView(scrollViewRect, scroll, scrollViewRect);
			
			for(int i = 0; i < buttons.Length; i++)
			{
				Texture texture = buttons[i];
				
				Rect rect = new Rect(x, y, buttonSize, buttonHeight);
				if(GUIUtilities.DrawTextureButton(rect, texture))
				{
					OnClick(i);
				}
				
				if(additionalDataDrawer != null)
				{
					additionalDataDrawer(i, rect);
				}
				
				if(i % buttonsOnLine == buttonsOnLine - 1)
				{
					x = buttonsLeft;
					y += yOffset;
				}
				else
				{
					x += xOffset;
				}
			}
			
			GUI.EndScrollView();
		}
	}
}

