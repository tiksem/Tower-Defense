using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class GameMenu : MonoBehaviour
{	
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
	
	private bool isShown = false;
	private AudioListener audioListener;
	
	private void InitButtons()
	{
		int count = buttonsOrder.Length;
		Texture[] textures = new Texture[count];
		
		audioListener = Camera.main.GetComponent<AudioListener>();
		
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
		audioListener.enabled = !audioListener.enabled;
		UpdateSoundButtonsTextures(); 
	}
	
	private void SaveGame()
	{
		
	}
	
	private void EndGame()
	{
		
	}
	
	private Texture GetSoundButtonTexture()
	{
		return audioListener.enabled ? soundOn : soundOff;
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
			EndGame();
			break;
		}
	}
	
	void OnValidate()
	{
		InitButtons();
	}
	
	void Start()
	{
		InitButtons();
		buttons.onClick = OnButtonClick;
	}
	
	void Update()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			isShown = !isShown;
		}
	}
	
	void OnGUI()
	{
		if(isShown)
		{
			buttons.Draw();
		}
	}
}
