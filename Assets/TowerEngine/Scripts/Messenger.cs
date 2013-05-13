using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[RequireComponent (typeof(GUIText))]
public class Messenger : MonoBehaviour
{
	public float fadeDuration = 2.0f;
	public float fadeAfter = 5.0f;
	
	private bool isFadeStopped = false;
	
	private void StartFading()
	{
		StopCoroutine("Fade");
		Color color = guiText.material.color;
		color.a = 1.0f;
		guiText.material.color = color;
		StartCoroutine("Fade");
	}
	
	private IEnumerator Fade()
	{
		return Animations.CreateFade(gameObject, 0.0f, fadeDuration, fadeAfter);
	}
	
	public void ShowMessage(string message)
	{
		guiText.text = message;
		StartFading();
	}
	
	// Use this for initialization
	void Start()
	{
		Color color = guiText.material.color;
		color.a = 0.0f;
		guiText.material.color = color;
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
