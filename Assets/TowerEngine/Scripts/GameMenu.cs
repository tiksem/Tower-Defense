using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;

public class GameMenu : MonoBehaviour
{	
	private static GameMenu instance;
	
	public static GameMenu Instance
	{
		get
		{
			return instance;
		}
	}
	
	public enum ButtonAction
	{
		SAVE_GAME,
		END_GAME,
		SOUND
	}
	
	public enum State
	{
		LOADING,
		END_GAME_CONFIRMATION_MESSAGE,
		SAVE_GAME,
		MENU
	}
	
	public GUIButtonGrid buttons = new GUIButtonGrid();
	public GUIButtonGrid saveGameButtons = new GUIButtonGrid();
	public GUIStyle saveGameDateStyle;
	public Texture defaultSaveButtonTexture;
	public ButtonAction[] buttonsOrder = new ButtonAction[]{ButtonAction.SOUND, ButtonAction.SAVE_GAME, ButtonAction.END_GAME}; 
	public Texture soundOn;
	public Texture soundOff;
	public Texture saveGame;
	public Texture endGame;
	public Texture darkenBackground;
	public Texture loadingBackground;
	
	public Texture cancelButton;
	public float cancelButtonSize = 0.1f;
	public float cancelButtonBorder = 0.1f;
	
	public Texture menuButton;
	public float menuButtonWidth = 0.1f;
	public float menuButtonHeight = 0.1f;
	public float menuButtonX = 0.0f;
	public float menuButtonY = 0.0f;
	public float timeScale = 1.0f;
	
	public string endGameMessage = "Exit game?";
	
	private bool isShown = false;
	private bool isLoading = false;
	private bool shouldShowEndGameConfirmationMessage = false;
	private bool hideAllControls = false;
	private State state = State.MENU;
	private SaveGameManager.SaveGameInfo[] saves;
	
	public Func<bool,Void> onMenuButtonClick;
	
	private void InitButtons()
	{
		int count = buttonsOrder.Length;
		Texture[] textures = new Texture[count];
		
		for(int i = 0; i < count; i++)
		{
			ButtonAction buttonAction = buttonsOrder[i];
			
			Texture texture = null;
			switch(buttonAction)
			{
			case ButtonAction.SOUND:
				texture = GetSoundButtonTexture();
				break;
			case ButtonAction.SAVE_GAME:
				texture = saveGame;
				break;
			case ButtonAction.END_GAME:
				texture = endGame;
				break;
			}
			
			textures[i] = texture;
		}
		
		buttons.SetButtons(textures);
	}
	
	private void UpdateSoundButtonsTextures()
	{
		Texture texture = GetSoundButtonTexture();
		
		int length = buttonsOrder.Length; 
		for(int i = 0; i < length; i++)
		{
			ButtonAction buttonAction = buttonsOrder[i];
			if(buttonAction == ButtonAction.SOUND)
			{
				buttons.SetButton(i, texture);
			}
		}
	}
	
	private void ToggleSound()
	{
		GameSettings.Instance.SoundEnabled = !GameSettings.Instance.SoundEnabled;
		UpdateSoundButtonsTextures();
	}
	
	public void PauseGame()
	{
		Time.timeScale = 0.0f;
	}
	
	public void ResumeGame()
	{
		Time.timeScale = timeScale;
	}
	
	public void HideAllControls()
	{
		hideAllControls = true;
		TowerManager.Instance.HideAllControls();
	}
	
	public bool IsLoading()
	{
		return isLoading;
	}
	
	public bool IsShown()
	{
		return isShown;
	}
	
	private void UpdateSaveGameButtons()
	{
		saves = SaveGameManager.instance.GetSaves();
		MainMenu.SetGUIButtonGridButtonsForLoadGame(saves, saveGameButtons, defaultSaveButtonTexture);
	}
	
	private void SaveGame()
	{
		UpdateSaveGameButtons();
		state = State.SAVE_GAME;
	}
	
	private void ShowEndGameConfirmationMessage()
	{
		MakeBackgroundDarken();
		GUIUtilities.MessageBoxResult result = GUIUtilities.DrawMessageBox(endGameMessage, GUIUtilities.MessageBoxType.YES_NO);
		if(result == GUIUtilities.MessageBoxResult.YES)
		{
			EndGame();
		}
		else if(result == GUIUtilities.MessageBoxResult.NO)
		{
			state = State.MENU;
		}
	}
	
	public void EndGame()
	{
		ResumeGame();
		Application.LoadLevelAsync(0);
		AdManager.instance.ShowEndOfTheRoundAds();
		isLoading = true;
	}
	
	private void OnEndGameClick()
	{
		state = State.END_GAME_CONFIRMATION_MESSAGE;
	}
	
	private Texture GetSoundButtonTexture()
	{
		return GameSettings.Instance.SoundEnabled ? soundOn : soundOff;
	}
	
	private void OnButtonClick(int index)
	{
		ButtonAction buttonAction = buttonsOrder[index];

		switch(buttonAction)
		{
		case ButtonAction.SOUND:
			ToggleSound();
			break;
		case ButtonAction.SAVE_GAME:
			SaveGame();
			break;
		case ButtonAction.END_GAME:
			OnEndGameClick();
			break;
		}
	}
	
	private bool DrawCancelButton()
	{
		bool result = GUIUtilities.DrawSquareButtonInRightTopCorner(cancelButton, cancelButtonSize, cancelButtonBorder);
		if(result)
		{
			GUIManager.Instance.SetMouseOverFlag(true);
		}
		
		return result;
	}
	
	private void MakeBackgroundDarken()
	{
		GUIUtilities.DrawBackground(darkenBackground);
	}
	
	private void DrawMenuButton()
	{
		bool fired = false;
		
		if(GUIUtilities.DrawTextureButton(menuButtonX, menuButtonY, menuButtonWidth, menuButtonHeight, menuButton))
		{
			fired = true;
			TriggerMenuVisibility();
			GUIManager.Instance.SetMouseOverFlag(true);
		}
		
		if(onMenuButtonClick != null)
		{
			onMenuButtonClick(fired);
		}
	}
	
	private void DrawMenu()
	{
		MakeBackgroundDarken();
		buttons.Draw();
		if(DrawCancelButton())
		{
			TriggerMenuVisibility();
		}
	}
	
	private void OnSaveGameCellClick(int index)
	{
		SaveGameManager.instance.SaveGame(index);
		TriggerMenuVisibility();
	}
	
	public static void DrawSaveGameCell(SaveGameManager.SaveGameInfo gameInfo, Rect rect, GUIStyle textStyle)
	{
		if(gameInfo == null)
		{
			return;
		}
		
		DateTime dateTime = gameInfo.dateTime;
		string dateText = dateTime.ToShortDateString();
		string timeText = dateTime.ToShortTimeString();
		
		rect.height /= 2;
		GUIUtilities.DrawText(rect, dateText, textStyle);
		rect.y += rect.height;
		GUIUtilities.DrawText(rect, timeText, textStyle);
	}
	
	private void DrawSaveGameCell(int index, Rect rect)
	{
		SaveGameManager.SaveGameInfo gameInfo = saves[index];
		DrawSaveGameCell(gameInfo, rect, saveGameDateStyle);
	}
	
	private void DrawSaveGameMenu()
	{
		MakeBackgroundDarken();
		saveGameButtons.Draw();
		if(DrawCancelButton())
		{
			state = State.MENU;
		}
	}
	
	private void DrawLoading()
	{
		GUIUtilities.DrawBackground(loadingBackground);
	}
	
	private void TriggerMenuVisibility()
	{
		state = State.MENU;
		isShown = !isShown;
		HandleIsShownParam();
	}
	
	private void HandleIsShownParam()
	{
		if(isShown)
		{
			TowerManager.Instance.HideAllControls();
			PauseGame();
		}
		else
		{
			TowerManager.Instance.ShowAllControls();
			ResumeGame();
		}
	}
	
	void OnValidate()
	{
		InitButtons();
	}
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		isShown = false;
		InitButtons();
		buttons.onClick = OnButtonClick;
		saveGameButtons.additionalDataDrawer = DrawSaveGameCell;
		saveGameButtons.onClick = OnSaveGameCellClick;
		GUIUtilities.CalculateFontSize(ref saveGameDateStyle);
		ResumeGame();
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.A))
		{
			if(!isShown || state == State.MENU)
			{
				TriggerMenuVisibility();
			}
			else
			{
				state = State.MENU;
			}
		}
	}
	
	void OnGUI()
	{
		GUIManager.Instance.SetMouseOverFlag(false);
		
		if(isLoading)
		{
			DrawLoading();
			return;
		}
		
		if(hideAllControls)
		{
			return;
		}
		
		if(isShown)
		{
			switch(state)
			{
			case State.MENU:
				DrawMenu();
				break;
			case State.END_GAME_CONFIRMATION_MESSAGE:
				ShowEndGameConfirmationMessage();
				break;
			case State.SAVE_GAME:
				DrawSaveGameMenu();
				break;
			}
		}
		else
		{
			DrawMenuButton();
		}
	}
}
