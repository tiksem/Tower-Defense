using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class MouseUtilities
	{	
		public static Vector3 GetMousePositionOnGameObject(GameObject gameObject, bool ignoreOtherObject = false)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			return Utilities.GetGameObjectRayIntersectionPoint(gameObject, ray, ignoreOtherObject);
		}
	}
}

