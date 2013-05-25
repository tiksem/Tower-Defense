using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MovingAndFadingGameObject : AnimatedObject
{
	public Vector3 translation = new Vector3(0, 1, 0);
	public float speed = 1.0f;
	public bool shouldFade = true;
	
	private MovingLerpInterpolator objectMoving;
	private Material material;
	
	public static MovingAndFadingGameObject AttachTo(GameObject gameObject, Vector3 translation, float speed)
	{
		MovingAndFadingGameObject that = gameObject.AddComponent<MovingAndFadingGameObject>();
		that.translation = translation;
		that.speed = speed;
		that.UpdateStartEndPostions();
		return that;
	}
	
	public void UpdateStartEndPostions()
	{
		objectMoving = MovingLerpInterpolator.FromTranslation(transform, translation, speed);
	}
	
	// Use this for initialization
	protected override void Start()
	{
		UpdateStartEndPostions();
		material = Animations.GetObjectMaterial(gameObject);
		base.Start();
	}
	
	protected override IEnumerator Animate()
	{
		float distancePart;
		while(objectMoving.MoveOneStep(out distancePart))
		{
			float fadeAlpha = 1.0f - distancePart;
			Color color = material.color;
			color.a = fadeAlpha;
			material.color = color;
			yield return new WaitForEndOfFrame();
		}
	}
}
