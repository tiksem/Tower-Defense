using UnityEngine;
using System.Collections;

public class GameSettings
{
	private static GameSettings instance;
	
	public  static GameSettings Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameSettings();
			}
			
			return instance;
		}
	}
	
	public bool SoundEnabled
	{
		get
		{
			return AudioListener.volume > 0.0f;
		}
		set
		{
			AudioListener.volume = value ? 1.0f : 0.0f;
		}
	}
}
