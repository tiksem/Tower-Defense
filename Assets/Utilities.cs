using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		
		public static float SelectValueFromRangeUsingValueInRange(float a, float b, float value, float fromA, float fromB)
		{
			if(value < fromA)
			{
				value = fromA;
			}
			
			if(value > fromB)
			{
				value = fromB;
			}
			
			float k = (a - b) / (fromA - fromB);
			return value * k + a;
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
				RaycastHit[] hits = Physics.RaycastAll(ray);
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
		
		public static RaycastHit FindFirstGameObjectHitInRay(GameObject[] gameObjects, Ray ray)
		{
			RaycastHit[] hits = Physics.RaycastAll(ray);
			return Array.Find(hits, (RaycastHit hit) => 
			{
				return Array.Find(gameObjects, (x) => x == hit.collider.gameObject);
			});
		}
		
		public static Vector3 FindFirstGameObjectIntersectionPointInRay(GameObject[] gameObjects, Ray ray)
		{
			RaycastHit raycastHit = FindFirstGameObjectHitInRay(gameObjects, ray);
			return raycastHit.point;
		}
		
		public static float RemoveModuloPart(float value, float divideBy)
		{
			return ((float)(int)(value / divideBy)) * divideBy;
		}
		
		public static float GetModuloPart(float value, float divideBy)
		{
			return value - RemoveModuloPart(value, divideBy);
		}
		
		public static bool IsPointInsideCircle(Vector2 point, Vector2 center, float circleRadius)
		{
			float sqrRadius = circleRadius * circleRadius;
			Vector2 dif = point - center;
			return sqrRadius >= dif.sqrMagnitude;
		}
		
		public static void SetActiveForAll(GameObject[] gameObjects, bool value)
		{
			foreach(GameObject gameObject in gameObjects)
			{
				if(gameObject != null)
				{
					gameObject.SetActive(value);
				}
			}
		}
		
		public static void DisableAll(GameObject[] gameObjects)
		{
			SetActiveForAll(gameObjects, false);
		}
		
		public static void EnableAll(GameObject[] gameObjects)
		{
			SetActiveForAll(gameObjects, true);
		}
		
		public static void SetActiveForAll(MonoBehaviour[] components, bool value)
		{
			foreach(MonoBehaviour component in components)
			{
				if(component != null)
				{
					component.gameObject.SetActive(value);
				}
			}
		}
		
		public static void DisableAll(MonoBehaviour[] components)
		{
			SetActiveForAll(components, false);
		}
		
		public static void EnableAll(MonoBehaviour[] components)
		{
			SetActiveForAll(components, true);
		}
		
		public static void SetActiveForAll(GUIElement[] components, bool value)
		{
			foreach(GUIElement component in components)
			{
				if(component != null)
				{
					component.gameObject.SetActive(value);
				}
			}
		}
		
		public static void DisableAll(GUIElement[] components)
		{
			SetActiveForAll(components, false);
		}
		
		public static void EnableAll(GUIElement[] components)
		{
			SetActiveForAll(components, true);
		}
		
		public static T[] GetGameObjectsInSphereWithComponent<T>(Vector3 position, float radius, int maxCount = int.MaxValue)
			where T : Component
		{
			Collider[] targetsColliders = Physics.OverlapSphere(position, radius);
			T[] components = new T[Math.Min(maxCount, targetsColliders.Length)];
		
			int componentIndex = 0;
			for(int i = 0; i < targetsColliders.Length && componentIndex < components.Length; i++)
			{
				GameObject gameObject = targetsColliders[i].gameObject;
				T component = gameObject.GetComponent<T>();
				if(component != null)
				{
					components[componentIndex++] = component;
				}
			}
			
			if(componentIndex == components.Length)
			{
				return components;
			}
			else if(componentIndex == 0)
			{
				return new T[0];
			}
			else
			{
				int componentsCount = componentIndex;
				T[] result = new T[componentsCount];
				Array.Copy(components, result, componentsCount);
				return result;
			}
		}
		
		public static T InstantiateAndGetComponent<T>(GameObject prefab, Vector3 position)
			where T : Component
		{
			GameObject gameObject = (GameObject)GameObject.Instantiate(prefab, position, prefab.transform.rotation);
			return gameObject.GetComponent<T>();
		}
		
		public static T InstantiateAndGetComponent<T>(GameObject prefab)
			where T : Component
		{
			return InstantiateAndGetComponent<T>(prefab, prefab.transform.position);
		}
		
		public static GameObject InstantiateAndDestroyAfter(GameObject prefab, Vector3 position, Quaternion rotation, float destroyAfter)
		{
			GameObject gameObject = (GameObject)GameObject.Instantiate(prefab, position, rotation);
			GameObject.Destroy(gameObject, destroyAfter);
			return gameObject;
		}
		
		public static GameObject InstantiateAndDestroyAfter(GameObject prefab, Vector3 position, float destroyAfter)
		{
			return InstantiateAndDestroyAfter(prefab, position, prefab.transform.rotation, destroyAfter);
		}
		
		public static void DestroyAll(IEnumerable objects)
		{
			foreach(UnityEngine.Object obj in objects)
			{
				GameObject.Destroy(obj);
			}
		}
		
		public static void DestroyAll(IList<UnityEngine.Object> objects)
		{
			DestroyAll(objects);
			objects.Clear();
		}
	}
}

