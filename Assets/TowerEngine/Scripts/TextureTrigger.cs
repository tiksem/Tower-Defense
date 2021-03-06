using UnityEngine;
using System.Collections;

[RequireComponent (typeof(GUITexture))]
public class TextureTrigger : MonoBehaviour
{
	public Texture2D alternateTexture;
	
	private Texture[] textures;
	GUITexture guiTexture;
	
	public void ChangeTexture(bool state)
	{
		int textureIndex = state ? 0 : 1;
		InitGuiTextureComponentIfNeed();
		guiTexture.texture = textures[textureIndex];
	}
	
	public bool GetState()
	{
		return guiTexture.texture != alternateTexture;
	}
	
	private void InitGuiTextureComponentIfNeed()
	{
		if(guiTexture == null)
		{
			guiTexture = GetComponent<GUITexture>();
		}
	}
	
	void OnValidate()
	{
		if(alternateTexture == null)
		{
			return;
		}
		
		InitGuiTextureComponentIfNeed();
	}
		
	// Use this for initialization
	void Start()
	{
		OnValidate();
		textures = new Texture[]{guiTexture.texture, alternateTexture};
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}
}
