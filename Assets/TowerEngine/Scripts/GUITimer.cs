using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;

public class GUITimer : Timer
{
	public float x;
	public float y;
	public GUIStyle textStyle;
	
	public Func<System.Void> onFinish;
	
	public static GUITimer CreateFromPrefab(GUITimer prefab, Func<Void> onFinish)
	{
		GUITimer timer = Utilities.InstantiateAndGetComponent<GUITimer>(prefab.gameObject);
		timer.onFinish = onFinish;
		return timer;
	}
	
	protected override void Start()
	{
		base.Start();
		GUIUtilities.CalculateFontSize(ref textStyle);
	}
	
	protected override void OnFinish()
	{
		if(onFinish != null)
		{
			onFinish();
		}
		
		base.OnFinish();
	}
	
	void OnGUI()
	{
		int currentTime = GetCurrentTime();
		GUIUtilities.DrawText(x, y, currentTime.ToString(), textStyle);
	}
}
