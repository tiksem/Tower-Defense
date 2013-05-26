using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[RequireComponent (typeof(GUIText))]
public class Messenger : MonoBehaviour
{
	private static Messenger instance;
	
	public static Messenger Instance
	{
		get
		{
			return instance;
		}
	}
	
	public float fadeDuration = 2.0f;
	public float fadeAfter = 5.0f;
	
	public string notEnoughGoldMessage = "Not enough gold";
	public string towerNotSelectedMessage = "Select tower";
	
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
		instance = this;
		Color color = guiText.material.color;
		color.a = 0.0f;
		guiText.material.color = color;
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
