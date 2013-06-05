using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;

public class TextWithIcon : MonoBehaviour
{
	public string text;
	
	public Texture icon;
	public float x = 0.1f;
	public float y = 0.1f;
	public float iconSize = 0.1f;
	
	public float textXOffset = 0.1f;
	public GUIStyle textStyle;
	
	public int IntText
	{
		get
		{
			return StringUtilities.ParseInt(text);
		}
		set
		{
			text = value.ToString();
		}
	}
	
	void OnValidate()
	{
		
	}
	
	void Start()
	{
		GUIUtilities.CalculateFontSize(ref textStyle);
	}
	
	void OnGUI()
	{
		GUIUtilities.DrawTextWithIcon(x, y, iconSize, icon, text, textStyle, textXOffset);
	}
}
