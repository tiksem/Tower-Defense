using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour
{
	private static GUIManager instance;
	
	private bool mouseOverFlag = false;
	
	private float lastMouseOverSetFlagTime = -1.0f;
	
	public void SetMouseOverFlag(bool value)
	{
		float currentTime = Time.time;
		
		if(!(!value && currentTime == lastMouseOverSetFlagTime))
		{
			mouseOverFlag = value;
		}
		
		lastMouseOverSetFlagTime = currentTime;
	}
	
	public bool GetMouseOverFlag()
	{
		return mouseOverFlag;
	}
	
	public static GUIManager Instance
	{
		get
		{
			return instance;
		}
	}
	
	void Start()
	{
		instance = this;
	}
}
