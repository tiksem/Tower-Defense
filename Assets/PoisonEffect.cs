using UnityEngine;
using System.Collections;

public class PoisonEffect : TargetHitEffect 
{
	public float duration = 10.0f;
	public float interval = 1.0f;
	public int damage = 1;
	public Weapon.AttackType attackType = Weapon.AttackType.POISON;
	
	private float startedTime = 0.0f;
	private float lastDamageTime = 0.0f;
	
	private void Damage()
	{
		targetComponent.Damage(attackType, damage);
	}
	
	public override bool CanBeAttachedToTarget(Target target)
	{
		return !target.HasImmunity(attackType);
	}
	
	protected override void ApplyToTarget()
	{
		float currentTime = Time.time;
		
		// avoid equals with 0.0f
		if(startedTime < 0.1f)
		{
			startedTime = currentTime;
			lastDamageTime = startedTime;
			return;
		}
		
		if(currentTime - startedTime > duration)
		{
			Destroy(gameObject);
			return;
		}
		
		if(currentTime - lastDamageTime >= interval)
		{
			Damage();
			lastDamageTime += interval;
		}
	}
}
