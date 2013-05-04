using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public struct ScreenRect
	{
		public Vector3 leftTop;
		public Vector3 leftBottom;
		public Vector3 rightTop;
		public Vector3 rightBottom;
	}
	
	public struct ScreenDiagonal
	{
		public Vector3 leftBottom;
		public Vector3 rightTop;
	}
	
	static public class CameraUtilities
	{
		public static Vector3 GetWorldVectorFromScreenPoints(Camera camera, Vector2 a, Vector2 b)
		{
			Vector3 worldA = ScreenToWorldPoint(camera, a);
			Vector3 worldB = ScreenToWorldPoint(camera, b);
			return worldA - worldB;
		}
		
		public static float GetWorldDistanceBetweenScreenPoints(Camera camera, Vector2 a, Vector2 b)
		{
			Vector3 vector = GetWorldVectorFromScreenPoints(camera, a, b);
			return vector.magnitude;
		}
		
		public static Vector3 ScreenToWorldPoint(Camera camera, Vector2 screenPoint)
		{
			Vector3 screenPoint3 = new Vector3(screenPoint.x, screenPoint.y, camera.nearClipPlane);
			return camera.ScreenToWorldPoint(screenPoint3);
		}
		
		public static ScreenRect GetCameraWorldRectPoints(Camera camera)
		{
			ScreenRect result = new ScreenRect();
			
			Vector3 screenPosition = new Vector3(0, 0, camera.nearClipPlane);
			result.leftBottom = camera.ScreenToWorldPoint(screenPosition);
			
			screenPosition.y = camera.pixelHeight;
			result.leftTop = camera.ScreenToWorldPoint(screenPosition);
			
			screenPosition.x = camera.pixelWidth;
			result.rightTop = camera.ScreenToWorldPoint(screenPosition);
			
			screenPosition.y = 0;
			result.rightBottom = camera.ScreenToWorldPoint(screenPosition);
			
			return result;
		}
		
		public static ScreenDiagonal GetCameraWorldDiagonalPoints(Camera camera)
		{
			ScreenDiagonal result = new ScreenDiagonal();
			
			Vector3 screenPosition = new Vector3(0, 0, camera.nearClipPlane);
			result.leftBottom = camera.ScreenToWorldPoint(screenPosition);
			
			screenPosition.y = camera.pixelHeight;			
			screenPosition.x = camera.pixelWidth;
			result.rightTop = camera.ScreenToWorldPoint(screenPosition);
			
			return result;
		}
		
		public static Vector3 WorldToNormalizedViewportPoint(Camera camera, Vector3 point) 
		{
    		// Use the default camera matrix to normalize XY,
    		// but Z will be distance from the camera in world units 
    		point = camera.WorldToViewportPoint(point);

    		if(camera.orthographic)
			{
        		// Convert world units into a normalized Z depth value
        		// based on orthographic projection
        		point.z = (2 * (point.z - camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) - 1f;

    		}
    		else
			{
        		// Convert world units into a normalized Z depth value35.
        		// based on perspective projection
        		point.z = ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) 
					+ (1/point.z) * (-2 * camera.farClipPlane * camera.nearClipPlane / (camera.farClipPlane - camera.nearClipPlane));       

    		}
			
    		return point;    

		}              

		public static Vector3 NormalizedViewportToWorldPoint(Camera camera, Vector3 point)
		{
    		if(camera.orthographic)
			{
        		// Convert normalized Z depth value into world units
        		// based on orthographic projection
        		point.z = (point.z + 1f) * (camera.farClipPlane - camera.nearClipPlane) * 0.5f + camera.nearClipPlane;
    		}
    		else 
			{
        		// Convert normalized Z depth value into world units
        		// based on perspective projection
        		point.z = ((-2 * camera.farClipPlane * camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane))
					/ (point.z - ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)));
    		}
			
    		// Use the default camera matrix which expects normalized XY but world unit Z        
    		return camera.ViewportToWorldPoint(point);    

		}
	}
}

