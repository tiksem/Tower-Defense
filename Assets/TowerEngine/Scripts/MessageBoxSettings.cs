using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MessageBoxSettings : MonoBehaviour
{
	public float width = 0.3f;
	public float height = 0.3f;
	public Texture background;
	public Texture close;
	public Texture cancel;
	public Texture ok;
	public float textBorderX = 0.1f;
	public float textBorderY = 0.1f;
	public float buttonSize;
	public float buttonOffsetY = 0.05f; 
	public float distanceBetweenButtons = 0.05f;
	
	public GUIStyle textStyle;
	
	void Start()
	{
		GUIUtilities.CalculateFontSize(ref textStyle);
		GUIUtilities.SetMessageBoxSettings(this);
	}
}
