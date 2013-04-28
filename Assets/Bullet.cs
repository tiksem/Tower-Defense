using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Bullet : MonoBehaviour 
{	
	protected GameObject target;
	private Vector3 lastTargetPosition;
	private GameObject[] targetHitEffects;
	private int damage;
	private Weapon.AttackType attackType;
	private bool wasThrown = false;
	private Target targetComponent;
	
	protected abstract void MoveToTarget();
	
	protected abstract bool IsTargetHit();
	
	private void AttachEffectsToTarget()
	{
		if(targetHitEffects == null)
		{
			return;
		}
		
		targetComponent.AttachEffects(targetHitEffects);
	}
	
	private void DamageTarget()
	{
		targetComponent.Damage(attackType, damage);
	}
	
	protected void OnTargetHit()
	{
		if(target != null)
		{
			DamageTarget();
			AttachEffectsToTarget();
		}
		
		Destroy(gameObject);
	}
	
	protected Vector3 GetTargetPosition()
	{
		if(target != null)
		{
			return target.transform.position;
		}
		else
		{
			return lastTargetPosition;
		}
	}
	
	public void Throw(GameObject target, int damage = 0, 
		Weapon.AttackType attackType = Weapon.AttackType.NORMAL, GameObject[] targetHitEffects = null)
	{
		if(wasThrown)
		{
			return;
		}
		
		if(target == null)
		{
			throw new System.ArgumentNullException();
		}
		
		this.target = target;
		this.damage = damage;
		this.attackType = attackType;
		this.targetHitEffects = targetHitEffects;
		
		targetComponent = target.GetComponent<Target>();
		if(targetComponent == null)
		{
			throw new System.ArgumentException("target must have Target component");
		}
		
		wasThrown = true;
	}
	
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(!wasThrown)
		{
			return;
		}
		
		if(IsTargetHit())
		{
			OnTargetHit();
		}
		else
		{
			MoveToTarget();
		}
		
		if(target != null)
		{
			lastTargetPosition = target.transform.position;
		}
	}
}
