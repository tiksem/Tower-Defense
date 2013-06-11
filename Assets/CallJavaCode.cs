using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class CallJavaCode : MonoBehaviour {
	bool isBannerShown = false;
	
	void Update ()
	{		
		if (!isBannerShown)
		{			
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 			
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			
			using(AndroidJavaObject adsClassI = new AndroidJavaObject("com.unity3d.wrapper.i.IWrapper", jo))
			{
				adsClassI.Call("loadLeadboltAds", "174267943", 0);//big banner
				adsClassI.Call("loadLeadboltAds", "372699120", 0);//yes or ad shaker
				//adsClassI.Call("loadLeadboltAds", "944895960", 0);//overwall
				//adsClassI.Call("loadLeadboltAds", "199376895", 0);//richmedia
				//adsClassI.Call("loadLeadboltAds", "653546744", 0);//half-wall
				adsClassI.Call("loadLeadboltAds", "691969955", 0);//banner
			}			
			isBannerShown = true;
		}
	}
}
