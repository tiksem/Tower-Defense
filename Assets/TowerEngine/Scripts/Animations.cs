using System;
using UnityEngine;
using System.Collections;

namespace AssemblyCSharp
{
	public static class Animations
	{	
		public static IEnumerator CreateFrameAnimation(Func<float, System.Void> callback, Func<bool> shouldBeStopped, 
			float duration, float executeAfter = 0.0f)
		{
			if(executeAfter > 0.0f)
			{
				yield return new WaitForSeconds(executeAfter);
			}
			
			float startTime = Time.time;
			float currentTime = startTime;
			float finishTime = startTime + duration;
			while(shouldBeStopped == null || !shouldBeStopped())
			{
				currentTime = Time.time;
				if(currentTime >= finishTime)
				{
					break;
				}
				else
				{
					float timeElapsed = currentTime - startTime;
					callback(timeElapsed);
					yield return new WaitForEndOfFrame();
				}
			}
		}
		
		public static Material GetObjectMaterial(GameObject gameObject)
		{
			Renderer renderer = gameObject.renderer;
			Material material;
			if(renderer != null)
			{
				material = renderer.material;
			}
			else
			{
				GUIText guiText = gameObject.guiText;
				if(guiText != null)
				{
					material = guiText.material;
				}
				else
				{
					throw new System.ArgumentException("gameObject should have Renderer or GUIText component to create fade");
				}
			}
			
			return material;
		}
		
		public static IEnumerator CreateFade(GameObject gameObject, float fadeTo, float duration, 
			 float executeAfter = 0.0f, Func<bool> shouldBeStopped = null)
		{
			Material material = GetObjectMaterial(gameObject);
			
			float startAlpha = material.color.a;
			float fadeDif = fadeTo - startAlpha;
			
			Func<float,System.Void> fadeFunction = (float timeElapsed) =>
			{
				Color color = material.color;
				color.a = startAlpha + fadeDif * timeElapsed / duration;
				material.color = color;
			};
			
			return CreateFrameAnimation(fadeFunction, shouldBeStopped, duration, executeAfter);
		}
	}
}

