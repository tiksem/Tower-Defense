using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour
{
	public int goldPrice;
	public int level;
	public int crystalPrice;
	public bool canSeeInvisibleUnits = false;
	public string name = "Unnamed";
	
	public TowerSkillsBar.TowerUpgrade[] upgrades;
	
	public virtual void NotifyNewTowerBuilt()
	{
		
	}
	
	public virtual void NotifySomeTowerDestroyed(Tower destroyedTower)
	{
		
	}
	
	// Use this for initialization
	protected virtual void Start()
	{
	
	}
}
