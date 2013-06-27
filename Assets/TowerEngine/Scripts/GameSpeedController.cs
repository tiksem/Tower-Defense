using UnityEngine;
using System.Collections;

public class GameSpeedController : GUIFloatWithPlusMinusButton
{
	public override void OnFloatValueChanged(float currentValue)
	{
		Time.timeScale = currentValue;
	}
	
	public override void OnGUI()
	{
		if(!GameMenu.Instance.IsLoading() && !GameMenu.Instance.IsShown())
		{
			base.OnGUI();
		}
	}
}
