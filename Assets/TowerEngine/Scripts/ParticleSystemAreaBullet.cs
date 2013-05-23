using UnityEngine;
using System.Collections;

[RequireComponent (typeof(ParticleSystem))]
public class ParticleSystemAreaBullet : AreaBullet
{
	private ParticleSystem particleSystem;
	
	protected override bool IsTargetHit()
	{
		return !particleSystem.IsAlive();
	}
	
	// Use this for initialization
	void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}
}
