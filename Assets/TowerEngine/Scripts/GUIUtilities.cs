using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class GUIUtilities
	{
		public static void ResizeGUITextureToFitScreenWidth(GUITexture guiTexture)
		{
			Rect pixelInset = guiTexture.pixelInset;
			float prevWidth = pixelInset.width;
			float width = pixelInset.width = Camera.main.pixelWidth;
			float k = width / prevWidth;
			pixelInset.height *= k;
			guiTexture.pixelInset = pixelInset;
		}
		
		public static GUITexture CreateGUITextureGameObject(Texture texture, Rect pixelInset, float x, float y)
		{
			GameObject buttonGameObject = new GameObject();
			GUITexture button = buttonGameObject.AddComponent<GUITexture>();
			button.texture = texture;
			buttonGameObject.transform.localScale = Vector3.zero;
			button.pixelInset = pixelInset;
			//buttonGameObject.transform.parent = gameObject.transform;
			buttonGameObject.transform.position = new Vector3(x, y, 0);
			return button;
		}
		
		public static Rect GetScreenRect()
		{
			return new Rect(0.0f, 0.0f, Camera.main.pixelWidth, Camera.main.pixelHeight);
		}
		
		public static void DrawBackground(Texture image)
		{
			Rect screenRect = GetScreenRect();
			GUI.DrawTexture(screenRect, image, ScaleMode.StretchToFill);
		}
		
		public static float ScreenXToGUIX(float x)
		{
			return x * Camera.main.pixelWidth;
		}
		
		public static float ScreenYToGUIY(float y)
		{
			return y * Camera.main.pixelHeight;
		}
		
		public static Rect ScreenToGUIRect(float x, float y, float width, float height)
		{
			Rect rect = new Rect();
			rect.x = ScreenXToGUIX(x);
			rect.y = ScreenYToGUIY(y);
			rect.width = ScreenXToGUIX(width);
			rect.height = ScreenYToGUIY(height);
			
			return rect;
		}
	}
}

