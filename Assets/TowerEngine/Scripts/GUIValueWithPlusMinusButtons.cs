using System;
using UnityEngine;
using AssemblyCSharp;

public class GUIValueWithPlusMinusButtons : MonoBehaviour
{
	public enum Orientation
	{
		VERTICAL,
		HORIZONTAL
	}
	
	public Texture plusButton;
	public Texture minusButton;
	public Texture disabledPlusButton;
	public Texture disabledMinusButton;
	
	public float x;
	public float y;
	public float textMargin;
	public float buttonWidth;
	public float buttonHeight;
	public string[] values = new string[1];
	public int startValueIndex = 0;
	public Orientation orientation = Orientation.VERTICAL;
	public GUIStyle textStyle;
		
	private int valueIndex;
	private float minusButtonX;
	private float minusButtonY;
	private float plusButtonX;
	private float plusButtonY;
	private float textWidth;
	private float textHeight;
	private float textX;
	private float textY;
	
	public virtual void OnValueChanged(string prevValue, string currentValue)
	{
		
	}
	
	public void InitValueIndex()
	{
		if(startValueIndex < 0)
		{
			startValueIndex = 0;
		}
		else if(startValueIndex >= values.Length)
		{
			startValueIndex = values.Length - 1;
		}
		valueIndex = startValueIndex;
	}
	
	private void InitTextSize()
	{
		GUIUtilities.CalculateMaxTextWidthAndHeight(out textWidth, out textHeight, values, textStyle);
	}
	
	public int GetValueIndex()
	{
		return valueIndex;
	}
	
	public void InitPositions()
	{
		InitTextSize();
		
		if(orientation == Orientation.HORIZONTAL)
		{
			minusButtonX = x;
			minusButtonY = y;
			textX = minusButtonX + buttonWidth + textMargin;
			textY = GUIUtilities.GetCentricCoordinateBySize(textHeight, buttonHeight) + y;
			plusButtonX = minusButtonX + buttonWidth + textMargin * 2 + textWidth;
			plusButtonY = y;
		}
		else
		{
			plusButtonX = x;
			plusButtonY = y;
			textX = GUIUtilities.GetCentricCoordinateBySize(textWidth, buttonWidth) + x;
			textY = plusButtonY + buttonHeight + textMargin;
			minusButtonX = x;
			minusButtonY = y + buttonHeight + textMargin * 2 + textHeight;
		}
	}
	
	public virtual void Start()
	{
		GUIUtilities.CalculateFontSize(ref textStyle);
		InitValueIndex();
		InitPositions();
	}
	
	private bool DrawMinusButton()
	{
		Texture texture = minusButton;
		if(disabledMinusButton != null)
		{
			if(valueIndex <= 0)
			{
				texture = disabledMinusButton;
			}
		}
		
		return GUIUtilities.DrawTextureButton(minusButtonX, minusButtonY, buttonWidth, buttonHeight, texture);
	}
	
	private void DrawText()
	{
		string text = values[valueIndex];
		GUIUtilities.DrawText(textX, textY, text, textStyle, textWidth, textHeight);
	}
	
	private bool DrawPlusButton()
	{
		Texture texture = plusButton;
		if(disabledPlusButton != null)
		{
			if(valueIndex >= values.Length - 1)
			{
				texture = disabledPlusButton;
			}
		}
		
		return GUIUtilities.DrawTextureButton(plusButtonX, plusButtonY, buttonWidth, buttonHeight, texture);
	}
	
	private void OnMinusButtonClick()
	{
		if(valueIndex <= 0)
		{
			return;
		}
		
		string prevValue = values[valueIndex];
		valueIndex--;
		string currentValue = values[valueIndex];
		OnValueChanged(prevValue, currentValue);
	}
	
	private void OnPlusButtonClick()
	{
		if(valueIndex >= values.Length - 1)
		{
			return;
		}
	
		string prevValue = values[valueIndex];
		valueIndex++;
		string currentValue = values[valueIndex];
		OnValueChanged(prevValue, currentValue);
	}
	
	public virtual void OnGUI()
	{
		DrawText();
		
		if(DrawMinusButton())
		{
			OnMinusButtonClick();
		}
		
		if(DrawPlusButton())
		{
			OnPlusButtonClick();
		}
	}
}

