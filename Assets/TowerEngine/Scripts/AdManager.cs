using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using AssemblyCSharp;

public class AdManager : MonoBehaviour
{
	public static AdManager instance;
	
	public string[] doNotShowAdsOnScenesWithName;
	public int[] doNotShowAdsOnScenesWithId;
	
	[System.Serializable]
	public class Ad
	{
		public string name;
		public int id;
		public bool showOnEndOfRound = false;
	}
	
	public Ad[] ads;
	
	bool isBannerShown = false;
	
	private void ShowAds(bool isEndOfTheRound)
	{
		if(!gameObject.activeSelf)
		{
			return;
		}
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 			
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			
		using(AndroidJavaObject adsClassI = new AndroidJavaObject("com.unity3d.wrapper.i.IWrapper", jo))
		{
			foreach(Ad ad in ads)
			{
				if(ad.showOnEndOfRound != isEndOfTheRound)
				{
					continue;
				}
					
				int id = ad.id;
				adsClassI.Call("loadLeadboltAds", id.ToString(), 0);
			}
		}	
	}
	
	public void ShowEndOfTheRoundAds()
	{
		ShowAds(true);
	}
	
	void Update ()
	{
		if (!isBannerShown)
		{
			if(doNotShowAdsOnScenesWithName.IndexOf(Application.loadedLevelName) >= 0)
			{
				return;
			}
			
			if(doNotShowAdsOnScenesWithId.IndexOf(Application.loadedLevel) >= 0)
			{
				return;
			}
			
#if UNITY_ANDROID && !UNITY_EDITOR
				
			ShowAds(false);
			isBannerShown = true;
#endif
		}
	}
	
	void Awake()
	{
		Utilities.InitSingleton(ref instance, this);
	}
}