using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public abstract class TargetHitEffectWithDuration : TargetHitEffect
{
	[System.Serializable]
	public class ColorChangeSettings
	{
		public bool changeColor = false;
		public Color changeTo = Color.white;
	}
	
	public float duration;
	public ColorChangeSettings colorChangeSettings;
	
	private float startTime;
	private Color prevColor;
	
	private void ChangeColor()
	{
		if(colorChangeSettings.changeColor)
		{
			prevColor = Rendering.GetMainColor(target);
			Rendering.SetMainColor(target, colorChangeSettings.changeTo);
		}
	}
	
	private void ReturnColor()
	{
		if(target != null && colorChangeSettings.changeColor)
		{
			Rendering.SetMainColor(target, prevColor);
		}
	}
	
	public override void OnDestroy()
	{
		ReturnColor();
	}
	
	protected override void FirstApplyToTarget()
	{
		ChangeColor();
		base.FirstApplyToTarget();
	}
	
	protected virtual void OnTimeReached()
	{
		Destroy(gameObject);
	}
	
	public override bool ShouldBeReplaced()
	{
		return true;
	}
	
	protected virtual void OnTimer()
	{
		
	}
	
	protected override sealed void ApplyToTarget()
	{
		float currentTime = Time.time;
		if(currentTime - startTime > duration)
		{
			OnTimeReached();
		}
		else
		{
			OnTimer();
		}
	}
	
	void Start()
	{
		startTime = Time.time;
	}
}
