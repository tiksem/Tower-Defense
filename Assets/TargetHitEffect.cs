using UnityEngine;
using System.Collections;

public abstract class TargetHitEffect : MonoBehaviour
{
	[HideInInspector]
	public GameObject target;
	
	public Weapon.AttackType attackType = Weapon.AttackType.NORMAL;
	
	protected Target targetComponent;
	
	private bool targetDetected = false;
	
	protected virtual void ApplyToTarget()
	{
		
	}
	
	protected virtual void FirstApplyToTarget()
	{
		
	}
	
	public virtual bool CanBeAttachedToTarget(Target target)
	{
		return !target.HasImmunity(attackType);
	}
	
	virtual public bool CanStack()
	{
		return false;
	}
	
	virtual public bool ShouldBeReplaced()
	{
		return false;
	}
	
	public void OnTargetDestroyed()
	{
		Destroy(gameObject);
	}
	
	public virtual void OnDestroy()
	{
		if(targetComponent != null)
		{
			targetComponent.NotifyEffectDestroyed(gameObject);
		}
	}
	
	private void InitTargetComponentIfNeed()
	{
		if(targetComponent == null)
		{
			targetComponent = target.GetComponent<Target>();
			if(targetComponent == null)
			{
				throw new System.ArgumentException("target must have Target component");
			}
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if(target != null)
		{
			InitTargetComponentIfNeed();
			
			if(!targetDetected)
			{
				FirstApplyToTarget();
			}
			targetDetected = true;
			
			ApplyToTarget();
		}
	}
}
