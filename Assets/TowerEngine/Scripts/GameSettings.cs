using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;

public class GameSettings
{
	private static GameSettings instance;
	
	private static readonly string SOUND_ENABLED_FILE_NAME = "sound_settings";
	
	public static GameSettings Instance
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
	
	private bool GetSoundEnabledFromFile()
	{
		try
		{
			return (bool)FileUtilities.Deserialize(SOUND_ENABLED_FILE_NAME);
		}
		catch(Exception e)
		{
			return true;
		}
	}
	
	private void SaveSoundEnabledToFile(bool value)
	{
		FileUtilities.Serialize(SOUND_ENABLED_FILE_NAME, value);
	}
	
	public void Init()
	{
		bool soundEnabled = GetSoundEnabledFromFile();
		AudioListener.volume = soundEnabled ? 1.0f : 0.0f;
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
			SaveSoundEnabledToFile(value);
		}
	}
}
