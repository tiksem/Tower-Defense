using UnityEngine;
using System.Collections;
using AssemblyCSharp;

[RequireComponent(typeof(Target))]
public class HealthBar : MonoBehaviour 
{
	public Color background = Color.black;
	public bool useGlobalColorHealthBarSettings = true;
	public Color[] colors = new Color[]
	{
		Color.blue,
		Color.green,
		Color.yellow,
		Color.red
	};
	
	public float height = 0.005f;
	public float width = 0.07f;
	public bool useGlobalSizeHealthBarSettings = true;
	public bool adjustHealthBarSizeByMeshBounds = true;
	public float yOffset = 0.0f;
	public float xOffset = 0.0f;
	
	
	private static readonly int TEXTURE_WIDTH = 100;
	private static readonly int TEXTURE_HEIGHT = 15;
	private static readonly int TEXTURE_BORDER = 2;
	
	private Target target;
	private int lastHP = -1;
	private Texture2D texture;
	private GUITexture guiTextureComponent;
	private GameObject guiTextureObject;
	private float pixelHeight;
	private float pixelWidth;
	private float pixelYOffset;
	private float pixelXOffset;
	private Bounds meshBounds;
	private Vector3 relativeMeshCenter;
	
	void InitWidthAndHeight()
	{
		if(colors.Length <= 0)
		{
			Debug.LogError("Colors length should not be 0");
		}
		
		if(useGlobalSizeHealthBarSettings)
		{
			if(HealthBarSettings.instance != null)
			{
				width = HealthBarSettings.instance.width;
				height = HealthBarSettings.instance.height;
			}
			else
			{
				Debug.LogError("Add HealthBarSettings to your scene");
			}
		}
		
		if(adjustHealthBarSizeByMeshBounds)
		{
			float size = meshBounds.size.magnitude;
			height *= size;
			width *= size;
		}
		
		pixelHeight = GUIUtilities.ScreenXToGUIX(height);
		pixelWidth = GUIUtilities.ScreenYToGUIY(width);
		pixelXOffset = GUIUtilities.ScreenXToGUIX(xOffset);
		pixelYOffset = GUIUtilities.ScreenYToGUIY(yOffset);
	}
	
	private void InitMeshTopAndBounds()
	{
		meshBounds = Rendering.GetObjectBounds(gameObject);
		relativeMeshCenter = meshBounds.center;
		relativeMeshCenter.x = meshBounds.min.x;
		relativeMeshCenter -= transform.position;
	}
	
	// Use this for initialization
	void Start()
	{	
		GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		target = GetComponent<Target>();
		texture = new Texture2D(TEXTURE_WIDTH, TEXTURE_HEIGHT);
		guiTextureObject = new GameObject();
		guiTextureComponent = guiTextureObject.AddComponent<GUITexture>();
		guiTextureComponent.texture = texture;
		
		InitMeshTopAndBounds();
		InitWidthAndHeight();
		
		guiTextureObject.SetActive(false);
	}
	
	private Color[] GetColors()
	{
		if(useGlobalColorHealthBarSettings)
		{
			if(HealthBarSettings.instance != null)
			{
				return HealthBarSettings.instance.colors;
			}
			
			Debug.LogError("Add HealthBarSettings to your scene");
		}
		
		return colors;
	}
	
	private Color GetColorFromHP()
	{
		Color[] colors = GetColors();
		
		int index = (int)((float)lastHP / (float)target.maxHP * (float)colors.Length);
		if(index >= colors.Length)
		{
			index = colors.Length - 1;
		}
		else if(index < 0)
		{
			index = 0;
		}
		
		return colors[index];
	}
	
	private bool IsBorder(int x, int y)
	{
		return x < TEXTURE_BORDER ||
			x >= TEXTURE_WIDTH - TEXTURE_BORDER ||
			y < TEXTURE_BORDER || 
			y >= TEXTURE_HEIGHT - TEXTURE_BORDER;
	}
	
	private void UpdateTexture()
	{
		Color hpColor = GetColorFromHP();
		int maxHP = target.maxHP;
		int hp = target.GetCurrentHP();
		int hpColorEnd = Utilities.ProjectFromOneRangeToAnother(hp, 0, TEXTURE_BORDER, maxHP, TEXTURE_WIDTH - TEXTURE_BORDER);
		
		for(int y = 0; y < TEXTURE_HEIGHT; y++)
		{
			for(int x = 0; x < TEXTURE_WIDTH; x++)
			{
				Color color = hpColor;
				
				if(IsBorder(x, y))
				{
					color = background;
				}
				else if(x - TEXTURE_BORDER > hpColorEnd)
				{
					color = background;
				}
				
				texture.SetPixel(x, y, color);
			}
		}
		
		texture.Apply();
	}
	
	private void UpdateTextureState()
	{
		int currentHP = target.GetCurrentHP();
		if(currentHP != lastHP)
		{
			lastHP = currentHP;
			UpdateTexture();
		}
	}
	
	private float GetDistanceFromCamera()
	{
		return Vector3.Distance(Camera.main.transform.position, transform.position);
	}
	
	private void DrawTexture()
	{
		Vector3 texturePosition = Camera.main.WorldToScreenPoint(relativeMeshCenter + transform.position);
		//Debug.Log(texturePosition);
		
		guiTextureObject.transform.rotation = transform.rotation;
		
		float textureMaxX = Camera.main.pixelWidth - pixelWidth;
		float textureMaxY = Camera.main.pixelHeight - pixelHeight;
		texturePosition.x = Utilities.ProjectFromOneRangeToAnother(texturePosition.x, 0.0f, 0.0f, textureMaxX, 1.0f);
		texturePosition.y = Utilities.ProjectFromOneRangeToAnother(texturePosition.y, 0.0f, 0.0f, textureMaxY, 1.0f);
		texturePosition.z = 0.0f;
		
		guiTextureObject.transform.localScale = Vector3.zero;
		guiTextureObject.transform.position = texturePosition;
		
		float xOffset = -pixelWidth / 2 + pixelXOffset; //Utilities.ProjectFromOneRangeToAnother(texturePosition.x, 0.0f, 0.0f, 1.0f, width);
		float yOffset = pixelHeight / 2 + pixelYOffset;//Utilities.ProjectFromOneRangeToAnother(texturePosition.y, 0.0f, 0.0f, 1.0f, height);
		
		guiTextureComponent.pixelInset = new Rect(xOffset, yOffset, pixelWidth, pixelHeight);

		guiTextureComponent.texture = texture;
	}
	
	void OnDestroy()
	{
		Destroy(guiTextureObject);
	}
	
	void Update()
	{
		if(!guiTextureObject.activeSelf)
		{
			if(target.WasDamaged())
			{
				guiTextureObject.SetActive(true);
			}
		}
		
		UpdateTextureState();
		DrawTexture();
	}
}
