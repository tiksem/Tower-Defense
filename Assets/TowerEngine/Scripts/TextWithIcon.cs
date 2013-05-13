using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GUIText))]
public class TextWithIcon : MonoBehaviour
{
	public GUITexture guiTexture;
	public float offset = 0.1f;
	
	//private 
	
	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		Vector2 pixelOffset = guiText.pixelOffset;
		pixelOffset.x = guiTexture.pixelInset.width + offset * Camera.main.pixelWidth;
		guiText.pixelOffset = pixelOffset;
	}
}
