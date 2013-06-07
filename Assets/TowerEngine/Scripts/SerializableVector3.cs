using System;
using UnityEngine;

namespace AssemblyCSharp
{
	[System.Serializable]
	public class SerializableVector3
	{
		public float x, y, z;
		
		public SerializableVector3(Vector3 vector)
		{
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}
		
		public Vector3 ToVector3()
		{
			return new UnityEngine.Vector3(x, y, z);
		}
	}
}

