using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class SplatPrototypeVisibilityTrigger
	{
		private SplatPrototype splatPrototype;
		private bool isShown = false;
		private Texture2D savedTexture;
		private static Texture2D transparentTexture;
		
		private static void InitTransparentTextureIfNeed()
		{
			if(transparentTexture == null)
			{
				transparentTexture = Utilities.CreateTransparentTexture();
			}
		}
		
		public SplatPrototypeVisibilityTrigger(SplatPrototype splatPrototype)
		{
			this.splatPrototype = splatPrototype;
			InitTransparentTextureIfNeed();
		}
		
		public bool IsShown()
		{
			return isShown;
		}
		
		public void Hide()
		{
			if(!isShown)
			{
				return;
			}
			
			savedTexture = splatPrototype.texture;
			splatPrototype.texture = transparentTexture;
			isShown = false;
		}
		
		public void Show()
		{
			if(isShown)
			{
				return;
			}
			
			
			splatPrototype.texture = savedTexture;
			isShown = true;
		}
	}
}

