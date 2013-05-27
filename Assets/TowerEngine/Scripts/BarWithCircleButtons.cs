using System;
using UnityEngine;
using AssemblyCSharp;
public class BarWithCircleButtons : MonoBehaviour
{
	public enum ButtonState
	{
		NORMAL,
		SELECTED,
		DISABLED
	}
	
	[System.Serializable]
	public class Button
	{
		public Texture normalState;
		public Texture selectedState;
		public Texture disabledState;
	}
	
	public float iconSize = 0.1f;
	public float firstIconLeft = 0.1f;
	public float iconBottom = 0.1f;
	public float distanceBetweenIcons = 0.1f;
		
	public Texture defaultButtonTexture;
	public Texture footerTexture;
	public float footerHeight = 0.1f;
	
	public int buttonsCount = 0;
	public Button[] buttonTextures;
	
	public Func<int,Void> onButtonClick;
		
	private Vector2[] centers;
	private ButtonState[] buttonsStates;
	private float iconY;
	private float iconHeight;
	private float iconWidth;
	private int clickedIndex = -1;
	
	private Texture GetTexture(int index)
	{
		ButtonState buttonState = buttonsStates[index];
		return GetTexture(index, buttonState);
	}
	
	private Texture GetTexture(int index, ButtonState buttonState)
	{
		if(index >= buttonTextures.Length)
		{
			return defaultButtonTexture;
		}
		
		Button button = buttonTextures[index];
			
		if(button == null)
		{
			return defaultButtonTexture;
		}
			
		Texture texture = button.normalState;
		if(buttonState == ButtonState.DISABLED)
		{
			if(button.disabledState != null)
			{
				texture = button.disabledState;
			}
		}
		else if(buttonState == ButtonState.SELECTED)
		{
			if(button.selectedState != null)
			{
				texture = button.selectedState;
			}
		}
		
		if(texture == null)
		{
			return defaultButtonTexture;
		}
		
		return texture;
	}
	
	public ButtonState GetButtonState(int buttonIndex)
	{
		return buttonsStates[buttonIndex];
	}
	
	public void SetButtonState(int buttonIndex, ButtonState buttonState)
	{
		buttonsStates[buttonIndex] = buttonState;
	}
	
	private void UpdateButtons()
	{
		buttonsStates = new ButtonState[buttonsCount];
		iconWidth = iconSize;
		iconHeight = GUIUtilities.GetHeightFromWidthForSquareButton(iconWidth);
		iconY = 1.0f - iconBottom - iconHeight;
	}
	
	private void DrawFooter()
	{
		GUIUtilities.DrawTextureScaledByWidthPlacedBottom(footerHeight, footerTexture);
	}
	
	private void DrawButtons()
	{
		clickedIndex = -1;
		
		float y = iconY;
		float x = firstIconLeft;
		float xStep = iconWidth + distanceBetweenIcons;
		
		for(int i = 0; i < buttonsCount; i++)
		{
			Texture texture = GetTexture(i);
			if(GUIUtilities.DrawTextureButton(x, y, iconWidth, iconHeight, texture))
			{
				clickedIndex = i;
			}
			
			x += xStep;
		}
		
		DrawFooter();

		if(clickedIndex >= 0)
		{
			if(onButtonClick != null)
			{
				onButtonClick(clickedIndex);
			}
			
			GUIManager.Instance.SetMouseOverFlag(true);
		}
	}
	
	public int GetButtonsCount()
	{
		return buttonsCount;
	}
	
	public void Start()
	{
		UpdateButtons();
	}
	
	public void OnGUI()
	{
		GUIManager.Instance.SetMouseOverFlag(false);
		DrawButtons();
	}
	
	public int GetClickedButtonIndex()
	{
		return clickedIndex;
	}
}
