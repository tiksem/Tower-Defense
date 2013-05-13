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
	private Vector3? initialPosition;
	
	public Vector3 GetInitialPosition()
	{
		if(initialPosition == null)
		{
			return transform.position;
		}
		else
		{
			return (Vector3)initialPosition;
		}
	}
	
	public void UpdatePosition()
	{
		Vector3 position = GetInitialPosition();
		
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
	
	// Use this for initialization
	void Start()
	{
		guiTexture = GetComponent<GUITexture>();
		initialPosition = transform.position;
		UpdatePosition();
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
