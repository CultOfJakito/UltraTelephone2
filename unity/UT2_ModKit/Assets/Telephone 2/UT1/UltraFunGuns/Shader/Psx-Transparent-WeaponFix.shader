Shader "psx/unlit/transparent/nocull-fresnel-weaponfix" {
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_OpacScale("Transparency Scalar", Range(0,1)) = 1
		_FresnelCol("Fresnel Color", Color) = (0.0, 0.25, 1, 1)
		_VertexWarpScale("Vertex Warping Scalar", Range(0,10)) = 1
	}
		SubShader{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" "PassFlags" = "OnlyDirectional"}
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			
			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#define UseStandardVert
				#define UseStandardFrag
				#define UseFresnel
				#define UseTransparency
				#include "Assets/CORE/Assets/Shader/PSX_Core.cginc"
				ENDCG
			}
		}
}