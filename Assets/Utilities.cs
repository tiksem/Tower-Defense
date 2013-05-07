using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class Utilities
	{
		public static GameObject InstantiateChildOf(GameObject prefab, GameObject parent)
		{
			GameObject child = (GameObject)GameObject.Instantiate(prefab);
			
			Vector3 initialPosition = prefab.transform.position;
			Quaternion initialRotation = prefab.transform.rotation;
			
			child.name = prefab.name;
			child.transform.parent = parent.transform;
			child.transform.localPosition = initialPosition;
			child.transform.localRotation = initialRotation;
			
			return child;
		}
		
		public static float ProjectFromOneRangeToAnother(float value, float from1, float to1, float from2, float to2) 
		{
			if(from1 > from2)
			{
				throw new System.ArgumentException("from1 > from2");	
			}
			
			if(to1 > to2)
			{
				throw new System.ArgumentException("to1 > to2");
			}
			
    		return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
		}
		
		public static int ProjectFromOneRangeToAnother(int value, int from1, int to1, int from2, int to2) 
		{
    		return (int)Math.Round(ProjectFromOneRangeToAnother((float)value, (float)from1, (float)to1, (float)from2, (float)to2));
		}
		
		public static Texture2D CreateTransparentTexture(int width = 2, int height = 2)
		{
			Texture2D texture = new Texture2D(width, height);
			Color color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{
					texture.SetPixel(x, y, color);
				}
			}
			
			texture.Apply();
			
			return texture;
		}
		
		public static Vector3 GetGameObjectRayIntersectionPoint(GameObject gameObject, Ray ray, bool ignoreOtherObjects = false)
		{
			int layerMask = gameObject.layer;
			if(!ignoreOtherObjects)
			{
				RaycastHit raycastHit = new RaycastHit();
				if(Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, layerMask))
				{
					if(raycastHit.collider.gameObject == gameObject)
					{
						return raycastHit.point;
					}
				}
				
				return Vector3.zero;
			}
			else
			{
				RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity, layerMask);
				foreach(RaycastHit hit in hits)
				{
					if(hit.collider.gameObject == gameObject)
					{
						return hit.point;
					}
				}
				
				return Vector3.zero;
			}
		}
		
		public static float RemoveModuloPart(float value, float divideBy)
		{
			return ((float)(int)(value / divideBy)) * divideBy;
		}
	}
}

