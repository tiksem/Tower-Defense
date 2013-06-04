using System;

namespace AssemblyCSharp
{
	public static class RandomUtilites
	{
		private static Random random = new Random();
		
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
	}
}

