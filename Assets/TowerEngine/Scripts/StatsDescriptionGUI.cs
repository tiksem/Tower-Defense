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
		public bool useHeadStyleForFooter = false;
		
		private GUIUtilities.DrawMessageBoxParams messageBoxParams;
		private float iconHeight;
		private float heightBorder;
		private float statsWidth;
		private float statsHeight;
		private float contentWidth;
		private float contentHeight;
		private float headTextWidth;
		private float headTextHeight;
		private float footerTextWidth;
		private float footerTextHeight;
		
		private void UpdateMessageBoxSettings()
		{
			messageBoxParams = new GUIUtilities.DrawMessageBoxParams();
			messageBoxParams.border = border;
			messageBoxParams.height = contentHeight;
			messageBoxParams.width = contentWidth;
			messageBoxParams.drawer = DrawContent;
		}
		
		public void ValidateTextStyles()
		{
			if(useHeadStyleForFooter)
			{
				footerTextStyle = headTextStyle;
			}
			
			GUIUtilities.CalculateFontSize(ref footerTextStyle);
			GUIUtilities.CalculateFontSize(ref headTextStyle);
		}
		
		private void InitContentSize()
		{
			iconHeight = GUIUtilities.GetHeightFromWidthForSquareButton(iconSize);
			heightBorder = GUIUtilities.GetHeightFromWidthForSquareButton(border);
			statsWidth = stats.GetContentWidth();
			statsHeight = stats.GetContentHeight();
			
			GUIUtilities.CalculateTextWidthAndHeight(out headTextWidth, out headTextHeight, headText, headTextStyle);
			GUIUtilities.CalculateTextWidthAndHeight(out footerTextWidth, out footerTextHeight, footerText, footerTextStyle);
			contentWidth = iconSize + border * 3 + statsWidth;
			contentHeight = headTextHeight + footerTextHeight + border * 4 + Math.Max(statsHeight, iconHeight);
		}
		
		public void Validate()
		{
			InitContentSize();
			UpdateMessageBoxSettings();
		}
		
		private void DrawContent(Rect rect)
		{
			float x = rect.x;
			float y = rect.y;
			
			y += heightBorder;
			
			x += border;
			GUIUtilities.DrawText(x, y, headText, headTextStyle, headTextWidth, headTextHeight);
			y += headTextHeight;
			y += heightBorder;
			
			GUIUtilities.DrawTexture(x, y, iconSize, iconHeight, icon);
			x += border;
			
			stats.x = x;
			stats.y = y;
			stats.Draw();
			y += Math.Max(statsHeight, iconHeight);
			y += heightBorder;
			
			x = rect.x + border;
			GUIUtilities.DrawText(x, y, footerText, footerTextStyle, footerTextWidth, footerTextHeight);
		}
		
		public GUIUtilities.MessageBoxResult DrawAsMessageBox()
		{
			return GUIUtilities.DrawMessageBoxUsingCustomDrawer(messageBoxParams, GUIUtilities.MessageBoxType.YES_NO);
		}
	}
}

