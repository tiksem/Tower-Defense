using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class BackgroundSoundsManager : MonoBehaviour
{
	public static BackgroundSoundsManager instance;
	
	[System.Serializable]
	public class BackgroundTrackPlayback
	{
		public AudioClip track;
		public float probability = 0.1f;
	}
	
	public AudioClip gameStartTrack;
	public BackgroundTrackPlayback[] backgroundTracks;
	
	private float[] soundPeekDistributionSteps;
	private AudioSource audioSource;
	
	private float PlayRandomSoundAndGetLength()
	{
		BackgroundTrackPlayback track = RandomUtilites.ChooseOneUsingDistributionFunctionSteps(backgroundTracks, soundPeekDistributionSteps);
		audioSource = AudioUtilities.PlayClipAt(track.track, Camera.main.transform.position);
		return track.track.length;
	}
	
	private void PlayMenuSound()
	{
		if(gameStartTrack != null)
		{
			audioSource = AudioUtilities.PlayClipAt(gameStartTrack, Camera.main.transform.position);
		}
	}
	
	private IEnumerator PlayRandomSounds()
	{
		if(backgroundTracks.Length > 0)
		{
			float soundLength = PlayRandomSoundAndGetLength();
			yield return new WaitForSeconds(soundLength);
			StartCoroutine(PlayRandomSounds());
		}
	}
	
	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
	
	void OnLevelWasLoaded(int index)
	{
		//MainMenu
		if(index == 0)
		{
			PlayMenuSound();
		}
		else
		{
			StartCoroutine(PlayRandomSounds());
		}
	}
	
	void Start()
	{
		backgroundTracks = Collections.Filter(backgroundTracks, (int index) => backgroundTracks[index].track != null);
		float[] probabilities = Collections.GetArrayFromArray<BackgroundTrackPlayback,float>
			(backgroundTracks, (BackgroundTrackPlayback track) => track.probability);
		soundPeekDistributionSteps = RandomUtilites.CreateDistributionFunctionSteps(probabilities);
		PlayMenuSound();
	}
	
	void Update()
	{
		if(audioSource != null)
		{
			audioSource.gameObject.transform.position = Camera.main.transform.position;
		}
	}
}
