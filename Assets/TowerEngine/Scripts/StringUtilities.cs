using System;

namespace AssemblyCSharp
{
	public static class StringUtilities
	{
		public static int ParseInt(string value, int failback = 0)
		{
			int result;
			if(int.TryParse(value, out result))
			{
				return result;
			}
			else
			{
				return failback;
			}
			
		}
	}
}

