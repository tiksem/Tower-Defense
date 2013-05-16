using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MainMenu : MonoBehaviour
{
	public Texture menuButtonTexture;
	public Texture background;
	public float distanceBetweenButtons;
	public float buttonsTop;
	public float buttonsLeft;
	public float buttonWidth;
	public float buttonHeight;
	public GUIStyle buttonStyle = new GUIStyle();
	public string[] buttonsText;
	
	private float buttonYOffset;
	
	void OnValidate()
	{
		buttonYOffset = buttonHeight + distanceBetweenButtons;
	}
	
	private Rect GetButtonRect(int index)
	{
		float y = buttonsTop + buttonYOffset * index;
		return GUIUtilities.ScreenToGUIRect(buttonsLeft, y, buttonWidth, buttonHeight);
	}
	
	private void OnButtonClick(int index)
	{
		
	}
		
	private void DrawButton(int index)
	{
		Rect rect = GetButtonRect(index);
		string text = buttonsText[index];
		
		if(GUI.Button(rect, text, buttonStyle))
		{
			OnButtonClick(index);
		}
	}
	
	private void DrawButtons()
	{
		if(buttonsText == null)
		{
			return;
		}
		
		for(int i = 0; i < buttonsText.Length; i++)
		{
			DrawButton(i);
		}
	}
	
	private void DrawBackground()
	{
		if(background != null)
		{
			GUIUtilities.DrawBackground(background);
		}
	}
	
	// Use this for initialization
	void Start()
	{
		
	}
	
	void OnGUI()
	{
		DrawBackground();
		DrawButtons();
	}
}
