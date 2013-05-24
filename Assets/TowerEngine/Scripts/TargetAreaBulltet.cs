using UnityEngine;
using System.Collections;

public class TargetAreaBulltet : ParticleSystemAreaBullet
{
	public float initialHeight = 3.0f;
	
	private bool positioned = false;
	
	private void SetInitalPosition()
	{
		Vector3 position = target.transform.position;
		position.y += initialHeight;
		transform.position = position;
		positioned = true;
	}
	
	private void SetInitalPositionIfNeed()
	{
		if(target != null)
		{
			SetInitalPosition();
		}
	}
	
	protected override void MoveToTarget()
	{
		SetInitalPositionIfNeed();
	}
	
	protected override Vector3 GetEpicenter()
	{
		return target.transform.position;
	}
}
