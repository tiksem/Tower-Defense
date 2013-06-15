using UnityEngine;
using System.Collections;

public class ArmorDecreaseEffect : TargetHitEffectWithDuration
{
	public int armorValue = 5;
	public bool canStack = true;
	
	private void ReturnArmor()
	{
		if(targetComponent != null)
		{
			targetComponent.CurrentPhysicalArmor += armorValue;
		}
	}
	
	protected override void FirstApplyToTarget()
	{
		base.FirstApplyToTarget();
		targetComponent.ChangePhysicalArmor(-armorValue, attackType);
	}
	
	public override void OnDestroy()
	{
		base.OnDestroy();
		ReturnArmor();
	}
	
	public override bool CanStack()
	{
		return canStack;
	}
}
