using UnityEngine;
using System.Collections;

public abstract class TargetHitEffect : MonoBehaviour
{
	[HideInInspector]
	public GameObject target;
	
	protected abstract void ApplyToTarget();
	
	public void OnTargetDestroyed ()
	{
		Destroy(gameObject);
	}
	
	// Use this for initialization
	void Start() 
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
		if(target != null)
		{
			ApplyToTarget();
		}
	}
}
