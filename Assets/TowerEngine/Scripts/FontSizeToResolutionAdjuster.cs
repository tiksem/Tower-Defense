using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class FontSizeToResolutionAdjuster : MonoBehaviour
{
	void Start()
	{
		AssemblyCSharp.GUIUtilities.CalculateFontSize(guiText);
	}
}
