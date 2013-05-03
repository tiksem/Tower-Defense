using UnityEngine;
using System.Collections;

public class Invisible : MonoBehaviour
{
	void Start()
	{
		if(renderer != null)
		{
			renderer.enabled = false;
		}
	}
	
	void Update()
	{
	
	}
}
