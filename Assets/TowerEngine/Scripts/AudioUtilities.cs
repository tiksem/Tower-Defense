using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public static class AudioUtilities
	{
		public static AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
		{
  			GameObject tempGO = new GameObject("TempAudio"); // create the temp object
  			tempGO.transform.position = pos; // set its position
  			AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
  			aSource.clip = clip; // define the clip
  			// set other aSource properties here, if desired
  			aSource.Play(); // start the sound
  			GameObject.Destroy(tempGO, clip.length); // destroy object after clip duration
  			return aSource; // return the AudioSource reference
		}
		
		public static AudioSource PlayClipAtCameraPosition(AudioClip clip)
		{
			return AudioUtilities.PlayClipAt(clip, Camera.main.transform.position);
		}
	}
}

