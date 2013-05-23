using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;

public abstract class AreaBullet : Bullet
{
	public float radius = 5.0f;
	public int maxTargetsCount = int.MaxValue;
	
	private Target[] GetTargets()
	{
		return Utilities.GetGameObjectsInSphereWithComponent<Target>(transform.position, radius, maxTargetsCount);
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
