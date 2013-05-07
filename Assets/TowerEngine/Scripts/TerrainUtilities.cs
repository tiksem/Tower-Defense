using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class TerrainUtilities
	{
		public static SplatPrototype GetSplatPrototypeByTextureName(Terrain terrain, string textureName)
		{
			SplatPrototype[] textures = terrain.terrainData.splatPrototypes;
			return Array.Find(textures, (SplatPrototype splatPrototype) => splatPrototype.texture.name == textureName );
		}
		
		public static Texture2D GetTextureByName(Terrain terrain, string textureName)
		{
			SplatPrototype splatPrototype = GetSplatPrototypeByTextureName(terrain, textureName);
			return splatPrototype.texture;
		}
	}
}

