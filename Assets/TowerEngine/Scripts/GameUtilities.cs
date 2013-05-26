using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class GameUtilities
	{
		private static float timeScaleBeforePauseGame = 1.0f;
		
		public static void PauseGame()
		{
			if(Time.timeScale != 0.0f)
			{
				timeScaleBeforePauseGame = Time.timeScale;
			}
			
			Time.timeScale = 0.0f;
		}
		
		public static void ResumeGame()
		{
			Time.timeScale = timeScaleBeforePauseGame != 0.0f ? timeScaleBeforePauseGame : 1.0f;
		}
	}
}

