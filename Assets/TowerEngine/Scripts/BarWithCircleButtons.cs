using System;
using UnityEngine;
using AssemblyCSharp;

	[RequireComponent(typeof(GUITexture))]
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
			public Texture2D normalState;
			public Texture2D selectedState;
			public Texture2D disabledState;
		}
	
		public float iconSize = 0.1f;
		public float firstIconLeft = 0.1f;
		public float iconBottom = 0.1f;
		public float distanceBetweenIcons = 0.1f;
		
		public Texture2D defaultButtonTexture;
		public int buttonsCount = 0;
		public Button[] buttonTextures;
		
		private GUITexture barTexture;
		private float buttonRadius;
		private float iconWidth;
		private float iconHeight;
		private GUITexture[] buttons;
		private Rect buttonPixelInset;
		private GuiEventsHandlerCombiner eventsHandler;
		private ButtonState?[] buttonsStates;
		
		private void InitTexture()
		{
			if(barTexture == null)
			{
				barTexture = GetComponent<GUITexture>();
			}
		}
			
		private void ResizeTexture()
		{
			GUIUtilities.ResizeGUITextureToFitScreenWidth(barTexture);
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
		
			return texture;
		}
		
		private GUITexture CreateButton(Texture texture, float x, float y)
		{
			return GUIUtilities.CreateGUITextureGameObject(texture, buttonPixelInset, x, y, -1);
		}
	
		public ButtonState GetButtonState(int buttonIndex)
		{
			return (ButtonState)buttonsStates[buttonIndex];
		}
	
		public void SetButtonState(int buttonIndex, ButtonState buttonState)
		{
			Texture texture = GetTexture(buttonIndex, buttonState);
			if(texture == null)
			{
				return;
			}
		
			buttons[buttonIndex].texture = texture;
			buttonsStates[buttonIndex] = buttonState;
		}
	
		public void UpdateButtons()
		{
			buttons = new GUITexture[buttonsCount];
			buttonsStates = new ButtonState?[buttonsCount];
		
			float screenWidth = Camera.main.pixelWidth;
			float screenHeight = Camera.main.pixelHeight;
			iconWidth = iconSize;
			float widthHeightCoefficient =  screenWidth / screenHeight;
			iconHeight = iconSize * widthHeightCoefficient;
			
			buttonPixelInset = new Rect();
			buttonPixelInset.width = iconWidth * screenWidth;
			buttonPixelInset.height = iconHeight * screenHeight;
			
			float x = firstIconLeft;
			float y = iconBottom * widthHeightCoefficient;
			float xStep = distanceBetweenIcons + iconWidth;
			for(int i = 0; i < buttonsCount; i++)
			{
				Texture texture = GetTexture(i, ButtonState.NORMAL);
				
				if(texture != null)
				{
					buttons[i] = CreateButton(texture, x, y);
					buttonsStates[i] = ButtonState.NORMAL;
				}
				
				x += xStep;
			}
		
			eventsHandler = new GuiEventsHandlerCombiner(buttons);
		}
	
		void OnDisable()
		{
			Utilities.DisableAll(buttons);
		}
	
		void OnEnable()
		{
			if(buttons != null)
			{
				Utilities.EnableAll(buttons);
			}
		}
	
		public void Start()
		{
			InitTexture();
			ResizeTexture();
			UpdateButtons();
		}
		
		public int GetClickedButtonIndex()
		{
			return eventsHandler.GetClickedGUIElementIndex();
		}
	}
