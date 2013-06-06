using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using AssemblyCSharp;

public class SaveGameManager : MonoBehaviour
{
	[System.Serializable]
	public class SaveGameInfo
	{
		public DateTime dateTime;
		public string sceneName;
	}
	
	public int maxSavesCount;
	
	private static int loadGameId = -1;
	
	public static SaveGameManager instance;
	
	private static Dictionary<string, object> GetSavingData()
	{
		try
		{
			return (Dictionary<string,object>)FileUtilities.Deserialize(loadGameId.ToString());
		}
		catch(Exception e)
		{
			return null;
		}
	}
	
	private static void SaveData(string fileName, object data)
	{
		FileStream file = new FileStream(fileName, FileMode.Create);
	}
	
	private void Restore()
	{
		if(loadGameId == -1)
		{
			return;
		}
		
		Dictionary<string,object> savingData = GetSavingData();
		if(savingData == null)
		{
			return;
		}
		
		UnityEngine.Object[] objects = GameObject.FindSceneObjectsOfType(typeof(MonoBehaviour)); 
		
		foreach(UnityEngine.Object obj in objects)
		{
			if(!(obj is SavingGameComponent))
			{
				continue;
			}
			
			GameObject gameObject = ((MonoBehaviour)obj).gameObject;
			SavingGameComponent saving = (SavingGameComponent)obj;
			object data = null;
			if(savingData.TryGetValue(gameObject.name, out data))
			{
				saving.OnRestore(data);
			}
		}
	}
	
	public static void StartGame(string sceneName)
	{
		loadGameId = -1;
		Application.LoadLevelAsync(sceneName);
	}
	
	public SaveGameInfo[] GetSaves()
	{
		SaveGameInfo[] saves = new SaveGameInfo[maxSavesCount];
		
		for(int i = 0; i < maxSavesCount; i++)
		{
			saves[i] = GetLevelInfoByLoadGameId(i);
		}
		
		return saves;
	}
	
	public static bool LoadGame(int id)
	{
		SaveGameInfo info = GetLevelInfoByLoadGameId(id);
		if(info == null)
		{
			return false;
		}
		
		loadGameId = id;
		Application.LoadLevelAsync(info.sceneName);
		return true;
	}
	
	private static string GetSceneNameFileByLoadGameId(int id)
	{
		return id.ToString() + "level";
	}
	
	public static SaveGameInfo GetLevelInfoByLoadGameId(int id)
	{
		try
		{
			string fileName = GetSceneNameFileByLoadGameId(id);
			return (SaveGameInfo)FileUtilities.Deserialize(fileName); 
		}
		catch(Exception e)
		{
			return null;
		}
	}
	
	public void SaveGame(int id)
	{
		SaveSceneInfo(id);
		SaveGameData(id.ToString());
	}
	
	private void SaveSceneInfo(int id)
	{
		string fileName = GetSceneNameFileByLoadGameId(id);
		
		SaveGameInfo saveGameInfo = new SaveGameInfo();
		saveGameInfo.sceneName = Application.loadedLevelName;
		saveGameInfo.dateTime = DateTime.Now;
		
		FileUtilities.Serialize(fileName, saveGameInfo);
	}
	
	private void SaveGameData(string fileName)
	{
		Dictionary<string,object> savingData = new Dictionary<string, object>();
		
		UnityEngine.Object[] objects = GameObject.FindSceneObjectsOfType(typeof(MonoBehaviour)); 
		
		foreach(UnityEngine.Object obj in objects)
		{
			if(!(obj is SavingGameComponent))
			{
				continue;
			}
			
			GameObject gameObject = ((MonoBehaviour)obj).gameObject;
			SavingGameComponent saving = (SavingGameComponent)obj;
			
			object data = saving.OnSave();
			if(data == null)
			{
				continue;
			}
			
			savingData.Add(gameObject.name, data);
		}
		
		FileUtilities.Serialize(fileName, savingData);
	}
	
	void Awake()
	{
		instance = this;
		Restore();
	}
}
