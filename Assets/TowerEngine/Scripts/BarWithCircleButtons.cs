using System;
using UnityEngine;
using AssemblyCSharp;

	[RequireComponent(typeof(GUITexture))]
	public class BarWithCircleButtons : MonoBehaviour
	{
		public float iconSize = 0.1f;
		public float firstIconLeft = 0.1f;
		public float iconBottom = 0.1f;
		public float distanceBetweenIcons = 0.1f;
		
		public Texture2D defaultButtonTexture;
		public Texture2D[] buttonTextures;
		
		private GUITexture barTexture;
		private float buttonRadius;
		private float iconWidth;
		private float iconHeight;
		private GUITexture[] buttons;
		private Rect buttonPixelInset;
		private GuiEventsHandlerCombiner eventsHandler;
		
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
		
		private Texture2D GetTexture(int index)
		{
			Texture2D texture = buttonTextures[index];
			
			if(texture == null)
			{
				return defaultButtonTexture;
			}
			
			return texture;
		}
		
		private GUITexture CreateButton(Texture2D texture, float x, float y)
		{
			return GUIUtilities.CreateGUITextureGameObject(texture, buttonPixelInset, x, y);
		}
		
		public void UpdateButtons()
		{
			buttons = new GUITexture[buttonTextures.Length];
		
			float screenWidth = Camera.main.pixelWidth;
			float screenHeight = Camera.main.pixelHeight;
			iconWidth = iconSize;
			float widthHeightCoefficient =  screenWidth / screenHeight;
			iconHeight = iconSize * widthHeightCoefficient;
			
			buttonPixelInset = new Rect();
			buttonPixelInset.width = iconWidth * screenWidth;
			buttonPixelInset.height = iconHeight * screenHeight;
			
			float x = firstIconLeft;
			float y = iconBottom;
			float xStep = distanceBetweenIcons + iconWidth;
			for(int i = 0; i < buttonTextures.Length; i++)
			{
				Texture2D texture = GetTexture(i);
				if(texture != null)
				{
					buttons[i] = CreateButton(texture, x, y);
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
