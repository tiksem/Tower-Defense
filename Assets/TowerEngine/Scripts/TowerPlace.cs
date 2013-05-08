using System;
using UnityEngine;

namespace AssemblyCSharp
{
	class TowerPlace
	{
		private float towerSize;	
		private Vector2 min;
		private Vector2 max;
		
		public TowerPlace(Bounds bounds, float towerSize)
		{
			this.towerSize = towerSize;
			
			min = new Vector2(bounds.min.x, bounds.min.z);
			max = new Vector2(bounds.max.x, bounds.max.z);
			
			float width = max.x - min.x;
			float height = max.y - min.y;
			
			width = Utilities.RemoveModuloPart(width, towerSize);
			height = Utilities.RemoveModuloPart(height, towerSize);
		}
		
		public bool IsIn(Vector2 position)
		{
			return position.x >= min.x && 
				position.x <= max.x && 
				position.y >= min.y && 
				position.y <= max.y;
		}
		
		public Vector2 CalculatePosition(Vector2 point)
		{
			float x = point.x - min.x;
			float y = point.y - min.y;
			x = Utilities.RemoveModuloPart(x, towerSize);
			y = Utilities.RemoveModuloPart(y, towerSize);
			
			return new Vector2(min.x + x, min.y + y);
		}
	}
}

