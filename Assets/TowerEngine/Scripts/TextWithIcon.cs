using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GUIText))]
public class TextWithIcon : MonoBehaviour
{
	public GUITexture guiTexture;
	public float offset = 0.1f;
	
	//private 
	
	private void TryAssignGUITextureIfNeed()
	{
		if(guiTexture == null)
		{
			Transform parent = transform.parent;
			if(parent == null)
			{
				return;
			}
			
			guiTexture = parent.gameObject.guiTexture;
		}
	}
	
	private void UpdatePosition()
	{
		if(guiTexture == null)
		{
			return;
		}
		
		Vector3 position =  guiTexture.transform.position + transform.position;
		position.x += guiTexture.pixelInset.width / Camera.main.pixelWidth + offset;
		transform.position = position;
	}
	
	// Use this for initialization
	void Start()
	{
		TryAssignGUITextureIfNeed();
	}
	
	// Update is called once per frame
	void Update()
	{
		UpdatePosition();
	}
}
