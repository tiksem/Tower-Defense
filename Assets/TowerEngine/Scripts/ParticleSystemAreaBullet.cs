using UnityEngine;
using System.Collections;

public class ParticleSystemAreaBullet : AreaBullet
{
	protected ParticleSystem particleSystem;
	
	protected override bool IsTargetHit()
	{
		return !particleSystem.IsAlive();
	}
	
	// Use this for initialization
	public virtual void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}
}
