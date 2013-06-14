using UnityEngine;
using System.Collections;

public class ProjectorAreaBullet : AreaBullet
{
	public Projector projectorPrefab;
	public float height = 6.0f;
	public float duration = 1.0f;
	public float maxFarClipPlane = 8.0f;
	
	private float startTime;
	private Projector projector;
	
	public virtual void Start()
	{
		Vector3 targetCenter = GetEpicenter();
		targetCenter.y += height;
		projector = (Projector)Instantiate(projectorPrefab, targetCenter, projectorPrefab.transform.rotation);
		startTime = Time.time;
		StartCoroutine(MoveProjector());
		projector.farClipPlane = 0.0f;
	}
	
	protected override bool IsTargetHit()
	{
		return Time.time - startTime >= duration;
	}
	
	private IEnumerator MoveProjector()
	{
		yield return new WaitForEndOfFrame();
		projector.farClipPlane = (Time.time - startTime) / duration * maxFarClipPlane;
	}
	
	protected virtual Vector3 GetEpicenter()
	{
		return target.collider.bounds.center;
	}
}
