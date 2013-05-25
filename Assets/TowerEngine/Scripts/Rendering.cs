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
	}
}

