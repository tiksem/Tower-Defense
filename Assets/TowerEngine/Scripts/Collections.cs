using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public static class Collections
	{
		public static T[] ClampArray<T>(T[] source, int length)
		{
			T[] clampedResult = new T[length];
			Array.Copy(source, clampedResult, length);
			return clampedResult;
		}
		
		public static To[] GetArrayFromArray<From, To>(From[] array, Func<From, To> transformer)
		{
			To[] to = new To[array.Length];
			for(int i = 0; i < to.Length; i++)
			{
				to[i] = transformer(array[i]);
			}
			
			return to;
		}
		
		public static T[] Filter<T>(T[] array, Func<int,bool> predicate)
		{
			T[] result = new T[array.Length];
			int index = 0;
			for(int i = 0; i < array.Length; i++)
			{
				if(predicate(i))
				{
					result[index++] = array[i];
				}
			}
			
			if(index == 0)
			{
				return new T[0];
			}
			
			if(index == array.Length)
			{
				return result;
			}
			
			return ClampArray(result, index);
		}
		
		public static T[] RemoveNulls<T>(T[] array)
		{
			return Filter(array, (int index) => array[index] == null);
		}
		
		public static float Sum(float[] arr)
		{
			float result = 0;
			for(int i = 0; i < arr.Length; i++)
			{
				result += arr[i];
			}
			
			return result;
		}
	}
}

