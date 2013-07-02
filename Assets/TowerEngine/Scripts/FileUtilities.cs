using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class FileUtilities
	{
		private static string persistentDataPath = UnityEngine.Application.persistentDataPath;
		
		public static string TransformFileName(string fileName)
		{
			return persistentDataPath + '/' + fileName;
		}
		
		public static void Delete(string fileName)
		{
			fileName = TransformFileName(fileName);
			File.Delete(fileName);
		}
		
		public static object Deserialize(string fileName)
		{
			try
			{
				fileName = TransformFileName(fileName);
				Stream stream = File.Open(fileName, FileMode.Open);
				BinaryFormatter binary = new BinaryFormatter();
            
				object result = binary.Deserialize(stream);
				stream.Close();
				return result;
			}
			catch(System.Exception e)
			{
				Debug.LogWarning(e);
				return null;
			}
		}
		
		public static bool Serialize(string fileName, object data)
		{
			try
			{
				fileName = TransformFileName(fileName);
				Stream stream = File.Open(fileName, FileMode.Create);
				BinaryFormatter binary = new BinaryFormatter();
            
				binary.Serialize(stream, data);
				stream.Close();
				
				return true;
			}
			catch(System.Exception e)
			{
				Debug.LogError(e);
				return false;
			}
		}
	}
}

