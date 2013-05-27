using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class PositionUtilities
	{
		public static void PutGameObjectCenterOnXZPlane(GameObject gameObject, Vector3 position, Bounds bounds)
		{
			Vector3 offset = bounds.extents;
			position.y += offset.y;
			gameObject.transform.position = position;
		}
		
		public static void PutGameObjectCenterOnXZPlane(GameObject gameObject, Vector3 position)
		{
			PutGameObjectCenterOnXZPlane(gameObject, position, gameObject.collider.bounds);
		}
		
		public static GameObject InstantiateGameObjectAndPutCenterOnXZPlane(GameObject prefab, Vector3 position, Bounds bounds)
		{
			GameObject gameObject = (GameObject)GameObject.Instantiate(prefab);
			PutGameObjectCenterOnXZPlane(gameObject, position, bounds);
			return gameObject;
		}
		
		public static GameObject InstantiateGameObjectAndPutCenterOnXZPlane(GameObject prefab, Vector3 position)
		{
			GameObject gameObject = (GameObject)GameObject.Instantiate(prefab);
			PutGameObjectCenterOnXZPlane(gameObject, position, gameObject.collider.bounds);
			return gameObject;
		}
		
		public static Vector2 XYZToXZ(GameObject gameObject)
		{
			return XYZToXZ(gameObject.transform.position);
		}
		
		public static Vector2 XYZToXZ(Vector3 position)
		{
			return new Vector2(position.x, position.z);
		}
	}
}

