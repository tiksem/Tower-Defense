Shader "Hidden/AddAlpha" { 
	Properties { 
		_MainTex ("Base (RGB)", 2D) = "white" {} 
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		AlphaTest Greater .01
		ColorMask RGB
		
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		Pass {
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex] {
				combine texture * previous DOUBLE
			}
		}
	} 
	FallBack off
}
