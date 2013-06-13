using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Rendering
	{
		public static Transform FindChildRecursive(Transform transform, string name)
		{
			Transform child = transform.FindChild(name);
			if(child != null)
			{
				return child;
			}
			
			int childCount = transform.GetChildCount();
			for(int i = 0; i < childCount; i++)
			{
				child = transform.GetChild(i);
				child = FindChildRecursive(child, name);
				if(child != null)
				{
					return child;
				}
			}
			
			return null;
		}
		
		public static Renderer GetRenderer(GameObject gameObject)
		{
			Renderer rendererComponent = gameObject.renderer;
			if(rendererComponent == null)
			{
				Transform body = FindChildRecursive(gameObject.transform, "body");
				
				if(body != null)
				{
					rendererComponent = body.gameObject.renderer;
				}
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
		
		public static void SetMainColor(GameObject gameObject, Color value)
		{
			Material material = Animations.GetObjectMaterial(gameObject);
			material.color = value;
		}
		
		public static Color GetMainColor(GameObject gameObject)
		{
			Material material = Animations.GetObjectMaterial(gameObject);
			return material.color;
		}
		
		public static void SetRenderingEnabled(GameObject gameObject, bool value)
		{
			Renderer renderer = GetRenderer(gameObject);
			renderer.enabled = value;
		}
		
		public static Vector3 GetBoundMaxByY(Bounds bounds)
		{
			if(bounds.max.y > bounds.min.y)
			{
				return bounds.max;
			}
			else
			{
				return bounds.min;
			}
		}
		
		public static Vector3 GetBoundMaxByY(GameObject gameObject)
		{
			Bounds bounds = GetObjectBounds(gameObject);
			return GetBoundMaxByY(bounds);
		}
		
		public static Vector3 GetObjectTopCenter(GameObject gameObject)
		{
			Bounds bounds = GetObjectBounds(gameObject);
			Vector3 top =  GetBoundMaxByY(gameObject);
			top.x = bounds.center.x;
			top.z = bounds.center.z;
			return top;
		}
		
		public static Vector3 GetObjectRelativeTopCenter(GameObject gameObject)
		{
			Bounds bounds = GetObjectBounds(gameObject);
			Vector3 top =  GetBoundMaxByY(gameObject);
			top.x = bounds.center.x;
			top.z = bounds.center.z;
			top -= bounds.min;
			return top;
		}
	}
}

