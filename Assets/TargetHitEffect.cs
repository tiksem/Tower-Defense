using UnityEngine;
using System.Collections;

public abstract class TargetHitEffect : MonoBehaviour
{
	[HideInInspector]
	public GameObject target;
	
	protected Target targetComponent;
	
	protected abstract void ApplyToTarget();
	
	public bool CanStack()
	{
		return false;
	}
	
	public void OnTargetDestroyed ()
	{
		Destroy(gameObject);
	}
	
	public void OnDestroy()
	{
		targetComponent.NotifyEffectDestroyed(gameObject);
	}
	
	// Use this for initialization
	void Start() 
	{
		
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
			ApplyToTarget();
		}
	}
}
