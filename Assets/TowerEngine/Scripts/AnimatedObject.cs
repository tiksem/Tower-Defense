using UnityEngine;
using System.Collections;

public abstract class AnimatedObject : MonoBehaviour
{
	public bool animateOnStart = true;
	public bool destroyWhenAnimationFinished = true;
	
	private bool animationStarted = false;
	private bool animationFinished = false;
	
	protected abstract IEnumerator Animate();
	
	private IEnumerator AnimationAction()
	{
		yield return StartCoroutine(Animate());
		animationFinished = true;
	}
	
	public void StartAnimation()
	{
		if(animationStarted)
		{
			return;
		}
		
		StartCoroutine("AnimationAction");
		animationStarted = true;
	}
	
	protected virtual void Start()
	{
		if(animateOnStart)
		{
			StartAnimation();
		}
	}
	
	protected virtual void Update()
	{
		if(animationFinished)
		{
			if(destroyWhenAnimationFinished)
			{
				Destroy(gameObject);
			}
		}
	}
}
