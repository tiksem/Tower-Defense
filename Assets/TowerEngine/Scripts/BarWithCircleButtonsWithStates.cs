using UnityEngine;
using System.Collections;

public class BarWithCircleButtonsWithStates : BarWithCircleButtons
{
	public int activeButtonsCount;
	public int selectedButtonIndex = 0;
	
	public void SetActiveButtonsCount(int activeButtonsCount)
	{
		this.activeButtonsCount = activeButtonsCount;
		if(activeButtonsCount > buttonsCount)
		{
			this.activeButtonsCount = buttonsCount;
		}
	}
	
	public override void Start()
	{
		base.Start();
		SetActiveButtonsCount(activeButtonsCount);
	}
	
	public void SelectButton(int index)
	{
		if(SetButtonStateToSelectedIfNotDisabled(index))
		{
			SetButtonState(selectedButtonIndex, BarWithCircleButtons.ButtonState.NORMAL);
			selectedButtonIndex = index;
		}
	}
	
	public override void OnGUI()
	{
		base.OnGUI();
		int index = GetClickedButtonIndex();
		if(index >= 0)
		{
			SelectButton(index);
		}
	}
}
