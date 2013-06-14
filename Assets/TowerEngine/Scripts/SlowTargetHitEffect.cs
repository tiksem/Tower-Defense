using UnityEngine;
using System.Collections;

public class SlowTargetHitEffect : TargetHitEffectWithDuration
{
	public float coefficient = 0.5f;
	
	private float speedBefore;
	private float angularSpeedBefore;
	
	public void OnValidate()
	{
		if(coefficient < 0)
		{
			coefficient = 0;
		}
	}
	
	private void ReturnStats()
	{
		if(targetComponent != null)
		{
			targetComponent.SetSpeeds(speedBefore, angularSpeedBefore);
		}
	}
	
	protected override void OnTimeReached()
	{
		ReturnStats();
		base.OnTimeReached();
	}
	
	public override void OnDestroy()
	{
		base.OnDestroy();
		ReturnStats();
	}
	
	protected override void FirstApplyToTarget()
	{
		speedBefore = targetComponent.GetSpeed();
		angularSpeedBefore = targetComponent.GetAngularSpeed();
		targetComponent.ChangeMovementSpeed(coefficient, attackType);
		base.FirstApplyToTarget();
	}
}
