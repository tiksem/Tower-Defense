using UnityEngine;
using System.Collections;
using AssemblyCSharp;

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
	
	public GUIButtonGrid buttons = new GUIButtonGrid();
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
	
	public string endGameMessage = "Exit game?";
	
	private bool isShown = false;
	private bool isLoading = false;
	private bool shouldShowEndGameConfirmationMessage = false;
	private bool hideAllControls = false;
	
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
		GameUtilities.PauseGame();
	}
	
	public void ResumeGame()
	{
		GameUtilities.ResumeGame();
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
	
	private void SaveGame()
	{
		
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
			shouldShowEndGameConfirmationMessage = false;
		}
	}
	
	public void EndGame()
	{
		ResumeGame();
		Application.LoadLevelAsync(0);
		isLoading = true;
	}
	
	private void OnEndGameClick()
	{
		shouldShowEndGameConfirmationMessage = true;
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
		return GUIUtilities.DrawSquareButtonInRightTopCorner(cancelButton, cancelButtonSize, cancelButtonBorder);
	}
	
	private void MakeBackgroundDarken()
	{
		GUIUtilities.DrawBackground(darkenBackground);
	}
	
	private void DrawMenuButton()
	{
		if(GUIUtilities.DrawTextureButton(menuButtonX, menuButtonY, menuButtonWidth, menuButtonHeight, menuButton))
		{
			TriggerMenuVisibility();
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
	
	private void DrawLoading()
	{
		GUIUtilities.DrawBackground(loadingBackground);
	}
	
	private void TriggerMenuVisibility()
	{
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
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.A))
		{
			TriggerMenuVisibility();
		}
	}
	
	void OnGUI()
	{		
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
			if(!shouldShowEndGameConfirmationMessage)
			{
				DrawMenu();
			}
			else
			{
				ShowEndGameConfirmationMessage();
			}
		}
		else
		{
			DrawMenuButton();
		}
	}
}
