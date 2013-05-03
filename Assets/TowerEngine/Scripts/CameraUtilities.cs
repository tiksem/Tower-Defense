using System;
using UnityEngine;

namespace AssemblyCSharp
{
	static public class CameraUtilities
	{
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

