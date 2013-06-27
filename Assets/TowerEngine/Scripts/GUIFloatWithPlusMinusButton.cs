using UnityEngine;
using System.Collections;

public class GUIFloatWithPlusMinusButton : GUIValueWithPlusMinusButtons
{
	public float[] floatValues = new float[1]{1};
	public string postfix;
	
	public virtual void Start()
	{
		int length = floatValues.Length;
		values = new string[length];
		for(int i = 0; i < length; i++)
		{
			values[i] = floatValues[i].ToString() + postfix;
		}
		
		base.Start();
	}
	
	public override sealed void OnValueChanged(string prevValue, string currentValue)
	{
		int index = GetValueIndex();
		float value = floatValues[index];
		OnFloatValueChanged(value);
	}
	
	public virtual void OnFloatValueChanged(float currentValue)
	{
		
	}
}
