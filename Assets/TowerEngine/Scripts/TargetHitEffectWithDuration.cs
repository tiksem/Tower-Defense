using UnityEngine;
using System.Collections;

public abstract class TargetHitEffectWithDuration : TargetHitEffect
{
	public float duration;
	
	private float startTime;
	
	protected virtual void OnTimeReached()
	{
		Destroy(gameObject);
	}
	
	public override bool ShouldBeReplaced()
	{
		return true;
	}
	
	protected virtual void OnTimer()
	{
		
	}
	
	protected override sealed void ApplyToTarget()
	{
		float currentTime = Time.time;
		if(currentTime - startTime > duration)
		{
			OnTimeReached();
		}
		else
		{
			OnTimer();
		}
	}
	
	void Start()
	{
		startTime = Time.time;
	}
}
