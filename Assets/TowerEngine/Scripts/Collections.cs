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
	}
}

