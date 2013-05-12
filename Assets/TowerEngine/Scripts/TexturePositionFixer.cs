using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[RequireComponent (typeof(GUITexture))]
public class TexturePositionFixer : MonoBehaviour
{
	public float textureBorder = 0.0f;
	public float textureSize = 0.2f;
	
	private GUITexture guiTexture;
	private float textureBorderX;
	private float textureBorderY;
	
	// Use this for initialization
	void Start()
	{
		guiTexture = GetComponent<GUITexture>();
		
		Vector3 position = transform.position;
		
		Rect pixelInset = guiTexture.pixelInset;
		float screenWidth = Camera.main.pixelWidth;
		float screenHeight = Camera.main.pixelHeight;
		
		float textureWidth = textureSize;
		float textureHeight = screenWidth / screenHeight * textureSize;
		
		pixelInset.width = textureWidth * screenWidth;
		pixelInset.height = textureHeight * screenHeight;
		guiTexture.pixelInset = pixelInset;
		
		textureBorderX = textureBorder;
		textureBorderY = textureBorder * textureHeight / textureWidth;
		
		position.x = Utilities.ProjectFromOneRangeToAnother(position.x, 0.0f, textureBorderX, 1.0f, 1.0f - textureWidth - textureBorderX);
		position.y = Utilities.ProjectFromOneRangeToAnother(position.y, 0.0f, textureBorderY, 1.0f, 1.0f - textureHeight - textureBorderY);
		
		transform.position = position;
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
