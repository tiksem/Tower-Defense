using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[RequireComponent(typeof(GUITexture))]
public class ToScreenWidthGUIResizer : MonoBehaviour
{
	public enum Align
	{
		NONE,
		TOP
	}
	
	public float height = 0.1f;
	public Align align = Align.NONE;
	
	// Use this for initialization
	void Start()
	{
		GUIUtilities.ResizeGUITextureToFitScreenWidth(guiTexture);
		Rect pixelInset = guiTexture.pixelInset;
		pixelInset.height = height * Camera.main.pixelHeight;
		guiTexture.pixelInset = pixelInset;
		
		if(align == Align.TOP)
		{
			Vector3 position = transform.position;
			position.y = 1.0f - height;
			transform.position = position;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
