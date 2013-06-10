using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Bullet : MonoBehaviour 
{
	public AudioClip creationSound;
	public AudioClip targetHitSound;
	public float volume = 0.4f;
	
	protected class TargetHit
	{
		public Target[] targets;
		public object data;
	}
	
	protected GameObject target;
	private Vector3 lastTargetPosition;
	private GameObject[] targetHitEffects;
	private int damage;
	private Weapon.AttackType attackType;
	private bool wasThrown = false;
	private Target targetComponent;
	private Renderer rendererComponent;
	
	protected abstract void MoveToTarget();
	
	protected abstract TargetHit CheckTargetHit();
	
	protected virtual void AttachEffectsToTarget(Target target)
	{
		if(targetHitEffects == null)
		{
			return;
		}
		
		target.AttachEffects(targetHitEffects);
	}
	
	private void PlaySound(AudioClip audio, Vector3 position)
	{
		if(audio == null)
		{
			return;
		}
		
		AudioSource.PlayClipAtPoint(audio, position, volume);
	}
	
	private void PlayCreationSound()
	{
		PlaySound(creationSound, transform.position);
	}
	
	private void PlayTargetHitSound(Vector3 position)
	{
		PlaySound(targetHitSound, position);
	}
	
	protected virtual void DamageTarget(Target target, int damage)
	{
		PlayTargetHitSound(target.transform.position);
		target.Damage(attackType, damage);
	}
	
	private void DamageTargets(Target[] targets)
	{
		foreach(Target target in targets)
		{
			DamageTarget(target, damage);
		}
	}
	
	protected virtual void AttachEffectsToTargets(Target[] targets)
	{
		foreach(Target target in targets)
		{
			AttachEffectsToTarget(target);
		}
	}
	
	private void InitTargetRenderer()
	{
		rendererComponent = target.renderer;
		if(rendererComponent == null)
		{
			Transform body = target.transform.FindChild("body");
			rendererComponent = body.gameObject.renderer;
		}
	}
	
	protected virtual void DestroyGameObject()
	{
		Destroy(gameObject);
	}
	
	protected void OnTargetHit(TargetHit targetHit)
	{
		if(targetHit.targets != null)
		{
			DamageTargets(targetHit.targets);
			AttachEffectsToTargets(targetHit.targets);
		}
		
		DestroyGameObject();
	}
	
	protected virtual Vector3 GetTargetCenter()
	{
		return rendererComponent.bounds.center;
	}
	
	protected Vector3 GetTargetPosition()
	{
		if(target != null)
		{
			return GetTargetCenter();
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
		
		InitTargetRenderer();
		PlayCreationSound();
		
		wasThrown = true;
	}
	
	protected TargetHit CreateTargetHitForCurrentTarget()
	{
		TargetHit targetHit = new TargetHit();
		targetHit.targets = new Target[]{target.GetComponent<Target>()};
		return targetHit;
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(!wasThrown)
		{
			return;
		}
		
		TargetHit targetHit = CheckTargetHit();
		if(targetHit != null)
		{
			OnTargetHit(targetHit);
		}
		else
		{
			MoveToTarget();
		}
		
		if(target != null)
		{
			lastTargetPosition = GetTargetCenter();
		}
	}
}
