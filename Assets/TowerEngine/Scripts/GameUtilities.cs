using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class GameUtilities
	{
		private static float timeScaleBeforePauseGame;
		
		public static void PauseGame()
		{
			timeScaleBeforePauseGame = Time.timeScale;
			Time.timeScale = 0.0f;
		}
		
		public static void ResumeGame()
		{
			Time.timeScale = timeScaleBeforePauseGame;
		}
	}
}

