using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class CallJavaCode : MonoBehaviour {
	void OnGUI ()
	{		
		if (GUI.Button(new Rect (60, 125, 200, 100), "Click to get Ads"))
		{	
		
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
					
			using(AndroidJavaObject adsClassI = new AndroidJavaObject("com.unity3d.wrapper.i.IWrapper", jo))
			{
				//988252279 banner
				//716330258 unlocker
				//324021230 audio
				adsClassI.Call("loadLeadboltAds", "988252279", 0);
			}
			
			using(AndroidJavaObject adsClassX = new AndroidJavaObject("com.unity3d.wrapper.x.XWrapper", jo))
			{
				//932367527 notification
				//138930168 icon
				adsClassX.Call("loadLeadboltAds", "138930168", 1);
			}

		}
	}
}
