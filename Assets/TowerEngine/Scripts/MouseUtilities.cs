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
	}
}

