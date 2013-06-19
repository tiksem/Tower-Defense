using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[System.Serializable]
	public class StatsDescriptionGUI
	{
		public GUIUtilities.Table stats;
		public Texture icon;
		public float iconSize;
		public float border;
		public string headText;
		public string footerText;
		
		public GUIStyle headTextStyle;
		public GUIStyle footerTextStyle;
		
		private GUIUtilities.DrawMessageBoxParams messageBoxParams;
		private float iconHeight;
		private float heightBorder;
		private float statsWidth;
		private float statsHeight;
		private float contentWidth;
		private float contentHeight;
		
		private void UpdateMessageBoxSettings()
		{
			messageBoxParams = new GUIUtilities.DrawMessageBoxParams();
			messageBoxParams.border = border;
			messageBoxParams.height = contentHeight;
			messageBoxParams.width = contentWidth;
			messageBoxParams.drawer = DrawContent;
		}
		
		private void InitContentWidth()
		{
			//GUIUtilities.CalculateTextWidthAndHeight(
		}
		
		public void Validate()
		{
			iconHeight = GUIUtilities.GetHeightFromWidthForSquareButton(iconSize);
			heightBorder = GUIUtilities.GetHeightFromWidthForSquareButton(border);
			statsWidth = stats.GetContentWidth();
			statsHeight = stats.GetContentHeight();
			UpdateMessageBoxSettings();
		}
		
		private void DrawContent(Rect rect)
		{
			float x = rect.x;
			float y = rect.y;
			
			y += heightBorder;
			
			x += border;
			Vector3 textSize = GUIUtilities.DrawText(x, y, headText, headTextStyle);
			y += textSize.y;
			y += heightBorder;
			
			GUIUtilities.DrawTexture(x, y, iconSize, iconHeight, icon);
			x += border;
			
			stats.x = x;
			stats.y = y;
			stats.Draw();
			y += Math.Max(statsHeight, statsHeight);
			y += heightBorder;
			
			x = rect.x + border;
			GUIUtilities.DrawText(x, y, footerText, footerTextStyle);
		}
		
		public GUIUtilities.MessageBoxResult DrawAsMessageBox()
		{
			return GUIUtilities.DrawMessageBoxUsingCustomDrawer(messageBoxParams, GUIUtilities.MessageBoxType.YES_NO);
		}
	}
}

