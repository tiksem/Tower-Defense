using UnityEngine;
using System.Collections;

public class GameSpeedController : GUIFloatWithPlusMinusButton
{
	public override void OnFloatValueChanged(float currentValue)
	{
		Time.timeScale = currentValue;
	}
}
