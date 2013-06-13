using UnityEngine;
using System.Collections;

public class HealthBarSettings : MonoBehaviour
{
	public static HealthBarSettings instance;
	
	public Color[] colors = new Color[]
	{
		Color.red,
		Color.yellow,
		Color.green,
		Color.blue,
	};
	
	public float height = 0.005f;
	public float width = 0.07f;
	
	void Awake()
	{
		instance = this;
	}
}
