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
		public float savingEngineVersion;
		public DateTime dateTime;
		public string sceneName;
	}
	
	public static readonly int AUTOSAVE_GAMEID = -2;
	
	public int maxSavesCount;
	public float savingEngineVersion = 1.0f;
	
	private int loadGameId = -1;
	private bool autosaveWasExecuted = false;
	
	public static SaveGameManager instance;
	
	private Dictionary<string, object> GetSavingData()
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
	
	private void SaveData(string fileName, object data)
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
	
	public AsyncOperation StartGame(string sceneName)
	{
		loadGameId = -1;
		return Application.LoadLevelAsync(sceneName);
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
	
	public bool WasAutoSaveExecuted()
	{
		return autosaveWasExecuted;
	}
	
	public AsyncOperation LoadAutosave()
	{
		return LoadGame(AUTOSAVE_GAMEID);
	}
	
	public AsyncOperation LoadGame(int id)
	{
		SaveGameInfo info = GetLevelInfoByLoadGameId(id);
		if(info == null)
		{
			return null;
		}
		
		loadGameId = id;
		return Application.LoadLevelAsync(info.sceneName);
	}
	
	private static string GetSceneNameFileByLoadGameId(int id)
	{
		return id.ToString() + "level";
	}
	
	public SaveGameInfo GetLevelInfoByLoadGameId(int id)
	{
		try
		{
			string fileName = GetSceneNameFileByLoadGameId(id);
			SaveGameInfo saveGameInfo = (SaveGameInfo)FileUtilities.Deserialize(fileName);
			if(saveGameInfo.savingEngineVersion != savingEngineVersion)
			{
				return null;
			}
			
			return saveGameInfo;
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
	
	public void ExecuteAutosave()
	{
		SaveGame(AUTOSAVE_GAMEID);
		autosaveWasExecuted = true;
	}
	
	public void ExecuteAutosaveAsync()
	{
		Dictionary<string,object> savingData = CreateSavingData();
		SaveGameInfo saveGameInfo;
		string fileName;
		GetSaveSceneInfo(AUTOSAVE_GAMEID, out fileName, out saveGameInfo);
		
		Func<Void> task = () => 
		{
			SaveSceneInfo(fileName, saveGameInfo);
			SaveGameData(AUTOSAVE_GAMEID.ToString(), savingData);
		};
		
		AsyncTaskManager.instance.RunOnBackground(task);
		
		autosaveWasExecuted = true;
	}
	
	public void DeleteSave(int id)
	{
		string fileName = GetSceneNameFileByLoadGameId(id);
		FileUtilities.Delete(fileName);
		FileUtilities.Delete(id.ToString());
	}
	
	public void DeleteAutosave()
	{
		DeleteSave(AUTOSAVE_GAMEID);
	}
	
	private void GetSaveSceneInfo(int id, out string fileName, out SaveGameInfo saveGameInfo)
	{
		fileName = GetSceneNameFileByLoadGameId(id);
		
		saveGameInfo = new SaveGameInfo();
		saveGameInfo.sceneName = Application.loadedLevelName;
		saveGameInfo.dateTime = DateTime.Now;
		saveGameInfo.savingEngineVersion = savingEngineVersion;
	}
	
	private void SaveSceneInfo(string fileName, SaveGameInfo saveGameInfo)
	{
		FileUtilities.Serialize(fileName, saveGameInfo);
	}
	
	private void SaveSceneInfo(int id)
	{
		SaveGameInfo saveGameInfo;
		string fileName;
		
		GetSaveSceneInfo(id, out fileName, out saveGameInfo);
		SaveSceneInfo(fileName, saveGameInfo);
	}
	
	private void SaveGameData(string fileName, Dictionary<string,object> savingData)
	{
		FileUtilities.Serialize(fileName, savingData);
	}
	
	private Dictionary<string,object> CreateSavingData()
	{
		UnityEngine.Object[] objects = GameObject.FindSceneObjectsOfType(typeof(MonoBehaviour));
		
		Dictionary<string,object> savingData = new Dictionary<string, object>();
		
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
		
		return savingData;
	}
	
	private void SaveGameData(string fileName)
	{
		Dictionary<string,object> savingData = CreateSavingData();
		SaveGameData(fileName, savingData);
	}
	
	void OnLevelWasLoaded(int levelIndex)
	{
		if(instance != this)
		{
			return;
		}
		
		if(levelIndex == 0)
		{
			//MainMenu level
			return;
		}
		
		Restore();
	}
	
	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
