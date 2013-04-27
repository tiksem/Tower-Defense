using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Bullet : MonoBehaviour 
{	
	[HideInInspector]
	public GameObject target;
	
	private List<GameObject> targetHitEffects = new List<GameObject>();
	private bool targetDefined = false;
	
	protected abstract void MoveToTarget();
	
	protected abstract bool IsTargetHit();
	
	private void CreateAndApplyEffectToTarget(GameObject effect)
	{
		TargetHitEffect effectComponent = effect.GetComponent<TargetHitEffect>();
		if(effectComponent == null)
		{
			Debug.LogError(effect.name + " is invalid: effect prefab must have TargetHitEffect component");
			return;
		}
		
		effect.transform.position = target.transform.position;
		effectComponent.target = target;
	}
	
	private void ApplyTargetHitEffects()
	{
		foreach(GameObject effect in targetHitEffects)
		{
			CreateAndApplyEffectToTarget(effect);
		}
	}
	
	protected void OnTargetHit()
	{
		ApplyTargetHitEffects();
		Destroy(gameObject);
	}
	
	void Start()
	{
		
	}
	
	public void AddTargetHitEffect(GameObject effect)
	{
		targetHitEffects.Add(effect);
	}
	
	private void DestroyEffects()
	{
		foreach(GameObject effect in targetHitEffects)
		{
			Destroy(effect);
		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(target == null)
		{
			if(targetDefined)
			{				
				DestroyEffects();
				Destroy(gameObject);
			}
			
			return;
		}
		else
		{
			targetDefined = true;
		}
		
		if(IsTargetHit())
		{
			OnTargetHit();
		}
		else
		{
			MoveToTarget();
		}
	}
}
