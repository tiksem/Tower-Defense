using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Rendering
	{
		public static Renderer GetRenderer(GameObject gameObject)
		{
			Renderer rendererComponent = gameObject.renderer;
			if(rendererComponent == null)
			{
				Transform body = gameObject.transform.FindChild("body");
				rendererComponent = body.gameObject.renderer;
			}
			
			return rendererComponent;
		}
		
		public static Bounds GetObjectBounds(GameObject gameObject)
		{
			Renderer renderer = GetRenderer(gameObject);
			if(renderer != null)
			{
				return renderer.bounds;
			}
			
			Collider collider = gameObject.collider;
			if(collider != null)
			{
				return collider.bounds;
			}
			
			throw new UnityEngine.MissingComponentException("the object '" + gameObject.name + "' has not bounds");
		}
		
		public static Vector3 GetObjectCenter(GameObject gameObject)
		{
			Bounds bounds = GetObjectBounds(gameObject);
			return bounds.center;
		}
		
		public static void SetAlpha(Material material, float value)
		{
			Color color = material.color;
			color.a = value;
			material.color = color;
		}
		
		public static void SetAlpha(GameObject gameObject, float value)
		{
			Material material = Animations.GetObjectMaterial(gameObject);	
			SetAlpha(material, value);
		}
		
		public static void SetRenderingEnabled(GameObject gameObject, bool value)
		{
			Renderer renderer = GetRenderer(gameObject);
			renderer.enabled = value;
		}
	}
}

