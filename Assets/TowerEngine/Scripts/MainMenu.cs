using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MainMenu : MonoBehaviour
{
	public enum ButtonAction
	{
		SINGLE_PLAYER
	}
	
	[System.Serializable]
	public class Button
	{
		public string text;
		public ButtonAction action;
	}
	
	[System.Serializable]
	public class Map
	{
		public string name;
		public Texture texture;
		public string sceneName;
	}
	
	[System.Serializable]
	public class Fraction
	{
		public string name;
		public Texture texture;
		public bool isDisabled = true;
	}
	
	public Texture background;
	public float distanceBetweenButtons;
	public float buttonsTop;
	public float buttonsLeft;
	public float buttonWidth;
	public float buttonHeight;
	public Button[] buttons;
	public Map[] maps;
	public int mapIndex;
	public Fraction[] fractions;
	public int fractionIndex;
	public float mapsButtonSize;
	public float chooseMapFractionButtonSize = 0.2f;
	public float chooseMapFractionButtonX = 0.2f;
	public float chooseMapButtonY = 0.2f;
	public float chooseFractionButtonY = 0.2f;
	public float startButtonWidth = 0.1f;
	public float startButtonHeight = 0.04f;
	public float startButtonX = 0.5f;
	public float startButtonY = 0.5f;
	public Texture startButtonTexture;
	public string startButtonText = "Start";
	public GUIStyle buttonStyle = new GUIStyle();
	
	
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
	
	private void OnSinglePlayerButtonClick()
	{
		
	}
	
	private void OnButtonClick(int index)
	{
		ButtonAction button = buttons[index].action;
		switch(button)
		{
		case ButtonAction.SINGLE_PLAYER:
			OnSinglePlayerButtonClick();
			break;
		}
	}
		
	private void DrawButton(int index)
	{
		Rect rect = GetButtonRect(index);
		Button button = buttons[index];
		string text = button.text;
		
		if(GUI.Button(rect, text, buttonStyle))
		{
			OnButtonClick(index);
		}
	}
	
	private void DrawButtons()
	{
		if(buttons == null)
		{
			return;
		}
		
		for(int i = 0; i < buttons.Length; i++)
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
	
	private void OnFractionChangeButtonClick()
	{
		
	}
	
	private void OnMapChangeButtonClick()
	{
		
	}
	
	private void DrawMapButton()
	{
		if(maps == null || maps.Length == 0)
		{
			return;
		}
		
		if(mapIndex >= maps.Length)
		{
			mapIndex = maps.Length - 1;
		}
		
		Texture mapTexture = maps[mapIndex].texture;
			
		float x = chooseMapFractionButtonX;
		float y = chooseMapButtonY;
		float width = chooseMapFractionButtonSize;	
		if(GUIUtilities.DrawSquareTextureButtonUsingWidth(x, y, width, mapTexture))
		{
			OnMapChangeButtonClick();
		}
	}
	
	private void DrawFractionButton()
	{
		if(fractions == null || fractions.Length == 0)
		{
			return;
		}
		
		if(fractionIndex >= fractions.Length)
		{
			fractionIndex = fractions.Length - 1;
		}
		
		Texture fractionTexture = fractions[fractionIndex].texture;
		
		float x = chooseMapFractionButtonX;
		float y = chooseFractionButtonY;
		float width = chooseMapFractionButtonSize;
		if(GUIUtilities.DrawSquareTextureButtonUsingWidth(x, y, width, fractionTexture))
		{
			OnFractionChangeButtonClick();
		}
	}
	
	private void OnStartButtonClick()
	{
		string sceneName = maps[mapIndex].sceneName;
		Application.LoadLevel(sceneName);
	}
	
	private void DrawStartButton()
	{
		if(startButtonTexture == null)
		{
			return;
		}
		
		Rect rect = GUIUtilities.ScreenToGUIRect(startButtonX, startButtonY, startButtonWidth, startButtonHeight);	
		if(GUI.Button(rect, startButtonText, buttonStyle))
		{
			OnStartButtonClick();
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
		DrawMapButton();
		DrawFractionButton();
		DrawStartButton();
	}
}
