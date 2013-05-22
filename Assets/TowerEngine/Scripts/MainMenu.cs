using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MainMenu : MonoBehaviour
{
	public enum ButtonAction
	{
		SINGLE_PLAYER,
		LOAD_GAME,
		OPTIONS,
		MULTIPLAYER,
		EXIT
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
	public float buttonTextSize = 1.0f;
	public float fontSizeForResolutionX = 960;
	
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
	
	public Texture backgroundDarkenTexture;
	public GUIButtonGrid mapPeekGridSettings;
	public GUIButtonGrid fractionPeekGridSettings;
	
	public ButtonAction selectedAction = ButtonAction.SINGLE_PLAYER;
	
	public Texture cancelButton;
	public float cancelButtonSize;
	public float cancelButtonBorder;
	public Texture multiplayerBackground;
	
	public Texture loadingBackground;
	
	public Texture gameLogo;
	public float gameLogoX = 0.0f;
	public float gameLogoY = 0.0f;
	public float gameLogoWidth = 0.1f;
	public float gameLogoHeight = 0.1f;
	
	public GUIStyle buttonStyle = new GUIStyle();
	public GUIStyle selectedButtonStyle = new GUIStyle();
	
	
	private float buttonYOffset;
	private float buttonFontSizeCoefficient;
	private bool backgroundShouldBeDarken = false;
	private bool shouldDrawMapPeekGrid = false;
	private bool shouldDrawFractionPeekGrid = false;
	private ButtonAction prevSelectedAction = ButtonAction.SINGLE_PLAYER;
	private AsyncOperation loadingOperation;
	
	private Texture[] GetMapsTextures()
	{
		Texture[] result = new Texture[maps.Length];
		for(int i = 0; i < result.Length; i++)
		{
			result[i] = maps[i].texture;
		}
		
		return result;
	}
	
	private Texture[] GetFractionsTextures()
	{
		Texture[] result = new Texture[fractions.Length];
		for(int i = 0; i < result.Length; i++)
		{
			result[i] = fractions[i].texture;
		}
		
		return result;
	}
	
	private void InitButtonFontSizeCoefficient()
	{
		buttonFontSizeCoefficient = Camera.main.pixelWidth / fontSizeForResolutionX;
	}
	
	private void CalculateFontSize(ref GUIStyle style)
	{
		style.fontSize = Mathf.RoundToInt(buttonFontSizeCoefficient * (float)style.fontSize);
	}
	
	private void FixButtonsFontSize()
	{
		CalculateFontSize(ref buttonStyle);
		CalculateFontSize(ref selectedButtonStyle);
	}
	
	void OnValidate()
	{
		buttonYOffset = buttonHeight + distanceBetweenButtons;
		
		if(maps != null)
		{
			Texture[] textures = GetMapsTextures();
			mapPeekGridSettings.SetButtons(textures);
		}
		
		if(fractions != null)
		{
			Texture[] textures = GetFractionsTextures();
			fractionPeekGridSettings.SetButtons(textures);
		}
	}
	
	private Rect GetButtonRect(int index)
	{
		float y = buttonsTop + buttonYOffset * index;
		return GUIUtilities.ScreenToGUIRect(buttonsLeft, y, buttonWidth, buttonHeight);
	}
	
	private void OnSinglePlayerButtonClick()
	{
		
	}
	
	private void OnMultiPlayerButtonClick()
	{
		backgroundShouldBeDarken = true;
	}
	
	private void OnOptionsButtonClick()
	{
		
	}
	
	private void OnExitButtonClick()
	{
		Application.Quit();
	}
	
	private void OnLoadGameButtonClick()
	{
		
	}
	
	private void HandleButtonClick(ButtonAction action)
	{
		if(action == selectedAction)
		{
			return;
		}
		
		prevSelectedAction = selectedAction;
		selectedAction = action;
		
		switch(action)
		{
		case ButtonAction.SINGLE_PLAYER:
			OnSinglePlayerButtonClick();
			break;
		case ButtonAction.MULTIPLAYER:
			OnMultiPlayerButtonClick();
			break;
		case ButtonAction.LOAD_GAME:
			OnLoadGameButtonClick();
			break;
		case ButtonAction.OPTIONS:
			OnOptionsButtonClick();
			break;
		case ButtonAction.EXIT:
			OnExitButtonClick();
			break;
		}
	}
	
	private void OnButtonClick(int index)
	{
		ButtonAction button = buttons[index].action;
		HandleButtonClick(button);
	}
	
	private void DrawButton(int index)
	{
		Rect rect = GetButtonRect(index);
		Button button = buttons[index];
		string text = button.text;
		
		GUIStyle style;
		if(selectedAction == button.action)
		{
			style = selectedButtonStyle;
		}
		else
		{
			style = buttonStyle;
		}
		
		if(GUI.Button(rect, text, style))
		{
			if(!backgroundShouldBeDarken)
			{
				OnButtonClick(index);
			}
		}
	}
	
	private void DrawButtons()
	{
		if(backgroundShouldBeDarken)
		{
			return;
		}
		
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
	
	private void MakeBackgroundDarkenIfRequired()
	{
		if(backgroundShouldBeDarken)
		{
			MakeBackgroundDarken();
		}
	}
	
	private void MakeBackgroundDarken()
	{
		if(backgroundDarkenTexture == null)
		{
			return;
		}
		
		GUIUtilities.DrawBackground(backgroundDarkenTexture);
	}
	
	private void OnFractionChangeButtonClick()
	{
		
	}
	
	private void OnMapChangeButtonClick()
	{
		backgroundShouldBeDarken = true;
		shouldDrawMapPeekGrid = true;
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
	
	private void DrawFractionPeekGrid()
	{
		fractionPeekGridSettings.Draw();
	}
	
	private void DrawMapPeekGrid()
	{
		mapPeekGridSettings.Draw();
	}
	
	private bool DrawCancelButton()
	{
		return GUIUtilities.DrawSquareButtonInRightTopCorner(cancelButton, cancelButtonSize, cancelButtonBorder);
	}
	
	private void HideAllGrids()
	{
		shouldDrawMapPeekGrid = false;
		backgroundShouldBeDarken = false;
		shouldDrawFractionPeekGrid = false;
	}
	
	private void OnMapPeek(int index)
	{
		mapIndex = index;
		HideAllGrids();
	}
	
	private void OnStartButtonClick()
	{
		Map map = maps[mapIndex];
		LoadMap(map);
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
		OnValidate();
		InitButtonFontSizeCoefficient();
		FixButtonsFontSize();
		mapPeekGridSettings.onClick = OnMapPeek;
	}
	
	private void OnSinglePlayer()
	{
		if(!backgroundShouldBeDarken)
		{
			DrawMapButton();
			DrawFractionButton();
			DrawStartButton();
		}
		
		if(shouldDrawMapPeekGrid)
		{
			DrawMapPeekGrid();
		}
		
		if(shouldDrawFractionPeekGrid)
		{
			DrawFractionPeekGrid();
		}
	}
	
	private void OnOptions()
	{
		
	}
	
	private void ReturnToPrevAction()
	{
		HideAllGrids();
		HandleButtonClick(prevSelectedAction);
	}
	
	private void DrawMiltiplayerBackground()
	{
		GUIUtilities.DrawBackground(multiplayerBackground);
	}
	
	private void OnMultiplayer()
	{
		if(cancelButton == null || multiplayerBackground == null)
		{
			return;
		}
		
		if(DrawCancelButton())
		{
			ReturnToPrevAction();
		}
		
		DrawMiltiplayerBackground();
	}
	
	private void OnLoadGame()
	{
		
	}
	
	private void LoadMap(Map map)
	{
		string levelName = map.sceneName;
		loadingOperation = Application.LoadLevelAsync(levelName);
	}
	
	private void DrawLoading()
	{
		GUIUtilities.DrawBackground(loadingBackground);
	}
	
	private bool DrawLoadingIfNeed()
	{
		if(loadingOperation != null && !loadingOperation.isDone)
		{
			DrawLoading();
			return true;
		}
		
		return false;
	}
	
	private void DrawGameLogo()
	{	
		if(gameLogo == null || backgroundShouldBeDarken)
		{
			return;
		}
		
		GUIUtilities.DrawTexture(gameLogoX, gameLogoY, gameLogoWidth, gameLogoHeight, gameLogo);
	}
	
	void OnGUI()
	{
		if(selectedAction == ButtonAction.EXIT)
		{
			return;
		}
		
		if(DrawLoadingIfNeed())
		{
			return;
		}
		
		DrawBackground();
		DrawButtons();
		MakeBackgroundDarkenIfRequired();
		DrawGameLogo();
		
		switch(selectedAction)
		{
		case ButtonAction.SINGLE_PLAYER:
			OnSinglePlayer();
			break;
		case ButtonAction.OPTIONS:
			OnOptions();
			break;
		case ButtonAction.MULTIPLAYER:
			OnMultiplayer();
			break;
		case ButtonAction.LOAD_GAME:
			OnLoadGame();
			break;
		}
	}
}
