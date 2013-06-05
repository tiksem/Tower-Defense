using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
	public int seconds = 10;
	public bool fireOnStart = true;
	public bool destroyOnFinish = true;
	
	private float startTime;
	private int currentTime = int.MinValue;
	
	public void StartTimer()
	{
		startTime = Time.time;
	}
	
	protected virtual void OnFinish()
	{
		if(destroyOnFinish)
		{
			Destroy(gameObject);
		}
	}
	
	protected virtual void OnTimeChanged(int prevTime, int newTime)
	{
		
	}
	
	public int GetCurrentTime()
	{
		return currentTime;
	}
	
	private void UpdateTimer()
	{
		if(currentTime == 0)
		{
			return;
		}
		
		float timeDif = Time.time - startTime;
		int prevTime = currentTime;
		currentTime = seconds - (int)timeDif;
		if(prevTime != currentTime)
		{
			OnTimeChanged(prevTime, currentTime);
		}
		
		if(currentTime <= 0)
		{
			OnFinish();
		}
	}
	
	protected virtual void Start()
	{
		if(fireOnStart)
		{
			StartTimer();
		}
	}
	
	void Update()
	{
		if(currentTime == int.MinValue)
		{
			currentTime = seconds;
		}
		
		UpdateTimer();
	}
}
