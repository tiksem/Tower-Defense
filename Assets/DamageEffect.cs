using UnityEngine;
using System.Collections;

public class DamageEffect : TargetHitEffect 
{
	public int damage = 1;
	
	private void Damage()
	{
		Target targetComponent = target.GetComponent<Target>();
		if(targetComponent == null)
		{
			Debug.LogError("target must have Target component");
			return;
		}
		
		targetComponent.Damage(attackType, damage);
	}
	
	// Update is called once per frame
	protected override void FirstApplyToTarget()
	{
		Damage();
		Destroy(gameObject);
	}
}
