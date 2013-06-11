using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class Brush : MonoBehaviour
{	
	[System.Serializable]
	public class BrushDefinition
	{
		public GameObject gameObject;
		public float period = 0.1f;
		public float destroyAfter = 1.0f;
	}
	
	public BarWithCircleButtonsWithStates bar;
	public BrushDefinition[] brushes;
	public Terrain terrain;
	public Texture clearButton;
	public float clearButtonSize;
	public float clearButtonBorder;
	
	private float lastObjectCreationTime = float.NegativeInfinity;
	private List<GameObject> objects = new List<GameObject>();
	
	private BrushDefinition GetBrush()
	{
		return brushes[bar.selectedButtonIndex];
	}
	
	void Start()
	{
		
	}
	
	private void CreateBrushParticle(GameObject gameObject, float destroyAfter)
	{
		Vector2 mousePosition = Input.mousePosition;
		Vector3 worldPosition = MouseUtilities.GetMousePositionOnGameObject(terrain.gameObject, true);
		gameObject = Utilities.InstantiateAndDestroyAfter(gameObject, worldPosition, destroyAfter);
		objects.Add(gameObject);
	}
	
	void Update()
	{
		if(GUIManager.Instance.GetMouseOverFlag())
		{
			return;
		}
		
		if(!Input.GetMouseButton(0))
		{
			return;
		}
		
		BrushDefinition brush = GetBrush();
		float currentTime = Time.time;
		if(brush.period <= currentTime - lastObjectCreationTime)
		{
			lastObjectCreationTime = currentTime;
			CreateBrushParticle(brush.gameObject, brush.destroyAfter);
		}
	}
	
	private bool DrawClearButton()
	{
		return GUIUtilities.DrawSquareButtonInRightTopCorner(clearButton, clearButtonSize, clearButtonBorder);
	}
	
	private void Clear()
	{
		Utilities.DestroyAll(objects);
	}
	
	void OnGUI()
	{
		if(DrawClearButton())
		{
			Clear();
		}
		
		GUIManager.Instance.SetMouseOverFlag(false);
	}
}
