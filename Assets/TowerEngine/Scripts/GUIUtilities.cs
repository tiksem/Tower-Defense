using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class GUIUtilities
	{
		private static Matrix4x4 pushedGUIMatrix;
		private static MessageBoxSettings messageBoxSettings;
		private static readonly float FONT_SIZE_RESOLUTION_X = 960;
		
		public static void SetMessageBoxSettings(MessageBoxSettings messageBoxSettings)
		{
			GUIUtilities.messageBoxSettings = messageBoxSettings;
		}
		
		public enum MessageBoxType
		{
			OK,
			YES_NO
		}
		
		public enum MessageBoxResult
		{
			NONE,
			OK,
			YES,
			NO
		}
		
		public static MessageBoxResult DrawMessageBox(string message, MessageBoxType type = MessageBoxType.OK)
		{
			if(messageBoxSettings == null)
			{
				throw new UnityEngine.MissingReferenceException("Add MessageBoxSettings gameObject to your scene");
			}
			
			float textBorderX = messageBoxSettings.textBorderX;
			float textBorderY = messageBoxSettings.textBorderY;
						
			float textWidth = -1.0f;
			float textHeight = -1.0f;
			AdjustTextWidthAndHeight(ref textWidth, ref textHeight, message, messageBoxSettings.textStyle);
			
			float textureWidth = Math.Max(messageBoxSettings.width, textWidth + textBorderX * 2);
			float textureHeight = Math.Max(messageBoxSettings.height, textHeight + textBorderY * 2);
			
			Vector2 xy = DrawTextureInCenter(textureWidth, textureHeight, messageBoxSettings.background);
			
			float textX = 0.5f - textWidth / 2;
			float textY = xy.y + textBorderY;
			
			DrawText(textX, textY, message, messageBoxSettings.textStyle, textWidth, textHeight);
			
			float buttonWidth = messageBoxSettings.buttonSize;
			float buttonHeight = GetHeightFromWidthForSquareButton(buttonWidth);
			float buttonY = xy.y + textureHeight - buttonHeight - messageBoxSettings.buttonOffsetY;
			
			MessageBoxResult messageBoxResult = MessageBoxResult.NONE;
			
			if(type == MessageBoxType.OK)
			{
				float buttonX = (1.0f - messageBoxSettings.buttonSize) / 2;
				if(DrawTextureButton(buttonX, buttonY, buttonWidth, buttonHeight, messageBoxSettings.ok))
				{
					messageBoxResult = MessageBoxResult.OK;
				}
			}
			else
			{
				float buttonX = xy.x + (textureWidth - buttonWidth * 2 - messageBoxSettings.distanceBetweenButtons) / 2;
				
				if(DrawTextureButton(buttonX, buttonY, buttonWidth, buttonHeight, messageBoxSettings.ok))
				{
					messageBoxResult = MessageBoxResult.YES;
				}
				
				buttonX += messageBoxSettings.distanceBetweenButtons + buttonWidth;
				if(DrawTextureButton(buttonX, buttonY, buttonWidth, buttonHeight, messageBoxSettings.cancel))
				{
					messageBoxResult = MessageBoxResult.NO;
				}
			}
			
			return messageBoxResult;
		}
		
		public static void ResizeGUITextureToFitScreenWidth(GUITexture guiTexture)
		{
			Rect pixelInset = guiTexture.pixelInset;
			float prevWidth = pixelInset.width;
			float width = pixelInset.width = Camera.main.pixelWidth;
			float k = width / prevWidth;
			pixelInset.height *= k;
			guiTexture.pixelInset = pixelInset;
		}
		
		public static GUITexture CreateGUITextureGameObject(Texture texture, Rect pixelInset, float x, float y, float z = 0.0f)
		{
			GameObject buttonGameObject = new GameObject();
			GUITexture button = buttonGameObject.AddComponent<GUITexture>();
			button.texture = texture;
			buttonGameObject.transform.localScale = Vector3.zero;
			button.pixelInset = pixelInset;
			//buttonGameObject.transform.parent = gameObject.transform;
			buttonGameObject.transform.position = new Vector3(x, y, z);
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
		
		public static bool DrawSquareTextureButtonUsingWidth(float x, float y, float width, Texture texture)
		{
			float height = width * Camera.main.pixelWidth / Camera.main.pixelHeight;
			return DrawTextureButton(x, y, width, height, texture);
		}
		
		public static bool DrawTextureButton(Rect rect, Texture texture)
		{
			return DrawTextureButton(rect.x, rect.y, rect.width, rect.height, texture);
		}
		
		public static bool DrawTextureButton(float x, float y, float width, float height, Texture texture)
		{
			Rect rect = ScreenToGUIRect(x, y, width, height);
			GUI.DrawTexture(rect, texture);
			return GUI.Button(rect, "", GUI.skin.label);
		}
		
		public static void DrawTextureScaledByWidth(float y, float height, Texture texture)
		{
			float x = 0;
			float width = 1.0f;
			DrawTexture(x, y, width, height, texture);
		}
		
		public static void DrawTextureScaledByWidthPlacedBottom(float height, Texture texture)
		{
			float y = 1.0f - height;
			DrawTextureScaledByWidth(y, height, texture);
		}
		
		public static bool DrawTextureButtonInLeftTopCorner(float width, float height, Texture texture)
		{
			float x = 0;
			float y = 0;
			return DrawTextureButton(x, y, width, height, texture);
		}
		
		public static void DrawTexture(float x, float y, float width, float height, Texture texture)
		{
			Rect rect = ScreenToGUIRect(x, y, width, height);
			GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill);
		}
		
		public static void DrawTexture(Vector2 xy, float width, float height, Texture texture)
		{
			DrawTexture(xy.x, xy.y, width, height, texture);
		}
		
		public static void AdjustTextWidthAndHeight(ref float width, ref float height, string text, GUIStyle textStyle)
		{
			GUIContent content = new GUIContent(text);
			Vector2 size = textStyle.CalcSize(content);
			if(width < 0)
			{
				width = size.x / Camera.main.pixelWidth;
			}
			if(height < 0)
			{
				height = size.y / Camera.main.pixelHeight;
			}
		}
		
		public static void DrawText(Rect rect, string text, GUIStyle textStyle)
		{
			DrawText(rect.x, rect.y, text, textStyle, rect.width, rect.height);
		}
		
		public static void DrawText(float x, float y, string text, GUIStyle textStyle, float width = -0.1f, float height = -0.1f)
		{
			AdjustTextWidthAndHeight(ref width, ref height, text, textStyle);
			Rect rect = ScreenToGUIRect(x, y, width, height);
			GUI.Label(rect, text, textStyle);
		}
		
		public static void DrawTextWithIcon(float x, float y, float iconSize, Texture icon, string text, GUIStyle textStyle, float textXOffset = 0.0f)
		{
			float textWidth = 0;
			float textHeight = 0;
			AdjustTextWidthAndHeight(ref textWidth, ref textHeight, text, textStyle);
			
			float iconHeight = GetHeightFromWidthForSquareButton(iconSize);
			float textX = x + iconSize + textXOffset;
			float textY = y + (iconHeight - textHeight) / 2;
			
			DrawTexture(x, y, iconSize, iconHeight, icon);
			DrawText(textX, textY, text, textStyle, textWidth, textHeight);
		}
		
		public static float ScreenXToGUIX(float x)
		{
			return x * Camera.main.pixelWidth;
		}
		
		public static float ScreenYToGUIY(float y)
		{
			return y * Camera.main.pixelHeight;
		}
		
		public static float GetHeightFromWidthForSquareButton(float width)
		{
			return width * Camera.main.pixelWidth / Camera.main.pixelHeight;
		}
		
		public static float GetCentricCoordinateBySize(float size, float maxSize = 1.0f)
		{
			if(size > maxSize || size < 0.0f)
			{
				throw new System.ArgumentException("size > maxSize || size < 0.0f");
			}
			
			return (maxSize - size) / 2;
		}
		
		public static Vector2 GetCentricScreenRectXY(float width, float height)
		{
			Vector2 result = new Vector2();
			result.x = GetCentricCoordinateBySize(width);
			result.y = GetCentricCoordinateBySize(height);
			return result;
		}
		
		public static Vector2 DrawTextureInCenter(float width, float height, Texture texture)
		{
			Vector2 xy = GetCentricScreenRectXY(width, height);
			DrawTexture(xy, width, height, texture);
			return xy;
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
		
		public static bool DrawButtonInRightTopCorner(Texture texture, float width, 
			float height, float buttonBorderWidth = 0.0f, float buttonBorderHeight = 0.0f)
		{
			float x = 1.0f - width - buttonBorderWidth;
			float y = buttonBorderHeight;
			return DrawTextureButton(x, y, width, height, texture);
		}
		
		public static bool DrawSquareButtonInRightTopCorner(Texture texture, float size, float border = 0.0f)
		{
			float heightK = Camera.main.pixelWidth / Camera.main.pixelHeight;
			float height = size * heightK;
			float heightBorder = border * heightK;
			
			return DrawButtonInRightTopCorner(texture, size, height, border, heightBorder);
		}
		
		public static void PushGUIScale(float precentageScale)
		{
			pushedGUIMatrix = GUI.matrix;
			
			Vector3 scale = Vector3.zero * precentageScale;
			Quaternion rotation = Quaternion.AngleAxis(0, new Vector3(0, 1, 0));
			Vector3 position = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 1);
			GUI.matrix = Matrix4x4.TRS(scale, rotation, position);
		}
		
		public static void PopGUIMatrix()
		{
			GUI.matrix = pushedGUIMatrix;
		}
	
		public static void CalculateFontSize(GUIText text)
		{
			float buttonFontSizeCoefficient = Camera.main.pixelWidth / FONT_SIZE_RESOLUTION_X;
			text.fontSize = Mathf.RoundToInt(buttonFontSizeCoefficient * (float)text.fontSize);
		}
		
		public static void CalculateFontSize(ref GUIStyle style)
		{
			float buttonFontSizeCoefficient = Camera.main.pixelWidth / FONT_SIZE_RESOLUTION_X;
			style.fontSize = Mathf.RoundToInt(buttonFontSizeCoefficient * (float)style.fontSize);
		}
		
		public static Vector3 WorldToGUIElementPositionPoint(Vector3 point)
		{
			point = Camera.main.WorldToScreenPoint(point);
			point.x /= Camera.main.pixelWidth;
			point.y /= Camera.main.pixelHeight;
			point.z = 0;
			return point;
		}
	}
}

