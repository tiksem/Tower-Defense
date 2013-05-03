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
	}
}

