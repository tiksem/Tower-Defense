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
		public string additionalText = "";
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
	public GUIStyle textStyle;
	public GUIStyle disabledTextStyle;
	public bool doNotUseDisabledTextStyle = false;
	public float textOffsetY = -0.1f;
	public float textOffsetX = 0.0f;
	
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
	
	public bool SetButtonStateToSelectedIfNotDisabled(int index)
	{
		ButtonState buttonState = GetButtonState(index);
		if(buttonState == ButtonState.DISABLED)
		{
			return false;
		}
		
		SetButtonState(index, ButtonState.SELECTED);
		return true;
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
	
	public string GetText(int index)
	{
		if(index >= buttonTextures.Length)
		{
			return "";
		}
		
		return buttonTextures[index].additionalText;
	}
	
	public void SetText(int index, object text)
	{
		buttonTextures[index].additionalText = text.ToString();
	}
	
	GUIStyle GetTextStyle(int index)
	{
		if(doNotUseDisabledTextStyle)
		{
			return textStyle;
		}
		
		ButtonState buttonState = GetButtonState(index);
		if(buttonState == ButtonState.DISABLED)
		{
			return disabledTextStyle;
		}
		else
		{
			return textStyle;
		}
	}
	
	private void DrawButtons()
	{
		clickedIndex = -1;
		
		DrawFooter();
		
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
			
			string text = GetText(i);
			
			if(text != "")
			{
				GUIStyle style = GetTextStyle(i);
				GUIUtilities.DrawText(x + textOffsetX, 1.0f - iconBottom + textOffsetY, text, style, iconWidth);
			}
			
			x += xStep;
		}

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
	
	public virtual void Start()
	{
		GUIUtilities.CalculateFontSize(ref textStyle);
		GUIUtilities.CalculateFontSize(ref disabledTextStyle);
		UpdateButtons();
	}
	
	public virtual void OnGUI()
	{
		GUIManager.Instance.SetMouseOverFlag(false);
		DrawButtons();
	}
	
	public int GetClickedButtonIndex()
	{
		return clickedIndex;
	}
}
