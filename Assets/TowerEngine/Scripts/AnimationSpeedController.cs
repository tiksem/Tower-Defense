using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AnimationSpeedController : MonoBehaviour
{
	public float animationSpeed = 1.0f;
	
	private NavMeshAgent navMeshAgent;
	private AnimationState run;
	
	// Use this for initialization
	void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		Animation animation = GetComponentInChildren<Animation>();
		
		if(animation != null)
		{
			run = animation["run"];
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if(run == null)
		{
			return;
		}
		
		run.speed = navMeshAgent.velocity.magnitude * animationSpeed;
	}
}
