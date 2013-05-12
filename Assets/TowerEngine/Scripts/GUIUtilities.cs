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
	}
}

