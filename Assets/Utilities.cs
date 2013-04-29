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
	}
}

