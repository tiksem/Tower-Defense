using UnityEngine;
using System.Collections;

public abstract class Bullet : MonoBehaviour {
	[HideInInspector]
	public GameObject target;
	
	protected abstract void MoveToTarget();
	
	protected abstract bool IsTargetHit();
	
	protected void OnTargetHit()
	{
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(target == null)
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
	}
}
