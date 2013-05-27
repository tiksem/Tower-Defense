using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[RequireComponent(typeof(GUITexture))]
[RequireComponent(typeof(GUITexturePercentageResizer))]
public class ObjectLabel : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;
	
	private GUITexture gui;
	private GUITexturePercentageResizer guiSize;
	
	void Start() 
    {
	    gui = guiTexture;
		guiSize = GetComponent<GUITexturePercentageResizer>();
	}
 
    void Update()
    {
 		if(target == null)
		{
			gui.enabled = false;
			return;
		}
		else
		{
			gui.enabled = true;
		}
		
		Vector3 position = Rendering.GetObjectTopCenter(target.gameObject);
		position = Camera.main.WorldToScreenPoint(position);
		position.x /= Camera.main.pixelWidth;
		position.x -= guiSize.width / 2;
		position.y /= Camera.main.pixelHeight;
		position += offset;
		
		transform.position = position;
    }
}