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
	
	void Awake()
	{
		instance = this;
	}
}
