using UnityEngine;
using System.Collections;

public class BuildTowerMenuButton : GuiEventsHandler
{
	protected override void OnClick()
	{
		TowerManager.Instance.OpenTowerBuildMenu();
	}
}
