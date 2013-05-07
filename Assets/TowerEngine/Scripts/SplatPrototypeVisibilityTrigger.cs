using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class SplatPrototypeVisibilityTrigger
	{
		private SplatPrototype splatPrototype;
		private bool isShown = true;
		private Texture2D savedTexture;
		private static Texture2D transparentTexture;
		private Terrain terrain;
		private int splatPrototypeIndex;
		
		private static void InitTransparentTextureIfNeed()
		{
			if(transparentTexture == null)
			{
				transparentTexture = Utilities.CreateTransparentTexture();
			}
		}
		
		public SplatPrototypeVisibilityTrigger(Terrain terrain, Texture2D texture)
		{
			SplatPrototype[] splatPrototypes = terrain.terrainData.splatPrototypes;
			int index = Array.FindIndex(splatPrototypes, 
				(SplatPrototype obj) => obj.texture == texture);
			
			if(index < 0)
			{
				throw new System.ArgumentException("could not find the texture in terrain splats");
			}
			
			this.terrain = terrain;
			splatPrototypeIndex = index;
			splatPrototype = splatPrototypes[index];
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
			
			UpdateTerrain();
		}
		
		public void UpdateTerrain()
		{
			TerrainData terrainData = terrain.terrainData;
			SplatPrototype[] splatPrototypes = terrainData.splatPrototypes;
			splatPrototypes[splatPrototypeIndex] = splatPrototype;
			terrainData.splatPrototypes = splatPrototypes;
			terrainData.RefreshPrototypes();
			terrain.terrainData = terrainData;
			terrain.Flush();
		}
		
		public void Show()
		{
			if(isShown)
			{
				return;
			}
			
			
			splatPrototype.texture = savedTexture;
			isShown = true;
			
			UpdateTerrain();
		}
	}
}

