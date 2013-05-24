using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;

public abstract class AreaBullet : Bullet
{
	public float radius = 5.0f;
	public int maxTargetsCount = int.MaxValue;
	
	protected virtual Vector3 GetEpicenter()
	{
		return transform.position;
	}
	
	private Target[] GetTargets()
	{
		if(target == null)
		{
			return null;
		}
		
		Vector3 center = GetEpicenter();
		return Utilities.GetGameObjectsInSphereWithComponent<Target>(center, radius, maxTargetsCount);
	}
	
	protected override void MoveToTarget()
	{
		
	}
	
	protected abstract bool IsTargetHit();
	
	protected override TargetHit CheckTargetHit()
	{
		if(IsTargetHit())
		{
			TargetHit targetHit = new TargetHit();
			targetHit.targets = GetTargets();
			return targetHit;
		}
		
		return null;
	}
}
