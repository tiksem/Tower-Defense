using UnityEngine;
using System.Collections;
using System;

public class FloatTimer : MonoBehaviour
{
	public float duration = 5.0f;
	public bool destroyOnFinish = true;
	public Func<Void> onFinish;
	
	private float startTime;
	
	public static FloatTimer AttachTo(GameObject gameObject, float duration)
	{
		FloatTimer floatTimer = gameObject.AddComponent<FloatTimer>();
		floatTimer.duration = duration;
		return floatTimer;
	}
	
	void Start()
	{
		startTime = Time.time;
	}
	
	void Update()
	{
		float currentTime = Time.time;
		if(currentTime >= startTime + duration)
		{
			if(onFinish != null)
			{
				onFinish();
			}
			
			if(destroyOnFinish)
			{
				Destroy(gameObject);
			}
		}
	}
}
