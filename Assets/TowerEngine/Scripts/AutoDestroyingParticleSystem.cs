using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyingParticleSystem : MonoBehaviour
{
	private ParticleSystem particleSystem;
	
	void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}
	
	void Update()
	{
		if(!particleSystem.IsAlive())
		{
			Destroy(gameObject);
		}
	}
}
