using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class RandomUtilites
	{
		private static System.Random random = new System.Random();
		
		public static bool Roll(float probability)
		{
			if(probability == 1.0f)
			{
				return true;
			}
			
			float randomValue = (float)random.NextDouble();
			return randomValue < probability;
		}
		
		private static float GetProbability(float[] probabilities, int index)
		{
			if(index < probabilities.Length)
			{
				return probabilities[index];
			}
			else
			{
				return 1.0f;
			}
		}
		
		public static T[] Roll<T>(T[] elements, float[] probabilities)
		{
			if(probabilities.Length == 0)
			{
				return elements;
			}
			
			return Collections.Filter(elements, (int index) =>
			{
				float probability = GetProbability(probabilities, index);
				return Roll(probability);
			});
		}
		
		public static float[] ValidateProbabilitiesSum(float[] probabilities)
		{
			float[] result = new float[probabilities.Length];
			float sum = Collections.Sum(result);
			
			for(int i = 0; i < probabilities.Length; i++)
			{
				if(sum != 0.0f)
				{
					result[i] = probabilities[i] / sum;
				}
				else
				{
					result[i] = 1.0f / (float)result.Length;
				}
			}
			
			return result;
		}
		
		public static float[] CreateDistributionFunctionSteps(float[] probabilities)
		{
			probabilities = ValidateProbabilitiesSum(probabilities);
			float[] steps = new float[probabilities.Length];
			float step = 0;
			
			for(int i = 0; i < probabilities.Length; i++)
			{
				step += probabilities[i];
				steps[i] = step;
			}
			
			return steps;
		}
		
		public static T ChooseOneUsingDistributionFunctionSteps<T>(T[] elements, float[] steps)
		{
			float value = (float)random.NextDouble();
			int index = Array.BinarySearch(steps, value);
			if(index < 0)
			{
				index = ~index;
			}
			
			if(index >= elements.Length)
			{
				index = elements.Length - 1;
			}
			
			return elements[index];
		}
	}
}

