using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GUIText))]
public class TextColor : MonoBehaviour
{
	public Color textColor = Color.black;
	
	void OnValidate()
	{
		if(Application.isPlaying)
		{
			guiText.material.color = textColor;
		}
	}
	
	// Use this for initialization
	void Start()
	{
		OnValidate();
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
