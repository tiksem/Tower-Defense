using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class MovingLerpInterpolator
	{
		Vector3 start;
		Vector3 end;
		float speed;
		float startTime;
		Transform transform;
		float distance;
		
		public static MovingLerpInterpolator FromTranslation(Transform transform, Vector3 translation, float speed = 1.0f)
		{
			Vector3 end = transform.position + translation;
			return new MovingLerpInterpolator(transform, end, speed);
		}
		
		public MovingLerpInterpolator(Transform transform, Vector3 end, float speed = 1.0f)
		{
			this.transform = transform;
			this.start = transform.position;
			this.end = end;
			startTime = Time.time;
			this.speed = speed;
			distance = Vector3.Distance(start, end);
		}
		
		private float GetPassedDistancePart()
		{
			float distancePassed = (Time.time - startTime) * speed;
			return distancePassed / distance;
		}
		
		public bool MoveOneStep(out float passedPart)
		{
			passedPart = GetPassedDistancePart();
			transform.position = Vector3.Lerp(start, end, passedPart);
			return transform.position != end;
		}
		
		public bool MoveOneStep()
		{
			float passedPart = 1.0f;
			return MoveOneStep(out passedPart);
		}
	}
}

