using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public static class MouseUtilities
	{
		public static Vector3 GetMousePositionOnGameObject(GameObject gameObject, bool ignoreOtherObject = false)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			return Utilities.GetGameObjectRayIntersectionPoint(gameObject, ray, ignoreOtherObject);
		}
		
		public static Vector3 FindFirstGameObjectIntersectionPointInMouseRay(GameObject[] gameObjects)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			return Utilities.FindFirstGameObjectIntersectionPointInRay(gameObjects, ray);
		}
		
		public static RaycastHit FindFirstGameObjectHitInMouseRay(GameObject[] gameObjects)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			return Utilities.FindFirstGameObjectHitInRay(gameObjects, ray);
		}
	}
}

