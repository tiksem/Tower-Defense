using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[RequireComponent(typeof(GUITexture))]
public class GUITexturePercentageResizer : MonoBehaviour
{
	public bool makeSquare = true;
	
	public float width = 0.1f;
	public float height = 0.1f;
	
	private void Resize()
	{
		if(makeSquare)
		{
			height = GUIUtilities.GetHeightFromWidthForSquareButton(width);
		}
		
		guiTexture.pixelInset = AssemblyCSharp.GUIUtilities.ScreenToGUIRect(0, 0, width, height);
	}
	
	// Use this for initialization
	void Start()
	{
		Resize();
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
