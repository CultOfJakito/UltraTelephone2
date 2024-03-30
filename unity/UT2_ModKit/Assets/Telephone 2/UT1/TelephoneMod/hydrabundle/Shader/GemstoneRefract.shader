Shader "Hydra/GemstoneRefraction" 
{

	Properties {
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend Mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend Mode", Float) = 10
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Blend Operator", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2
		
		_MainColor("Main Color", Color) = (1,1,1,1)
		_SecondaryColor("Secondary Color", Color) = (1,1,1,1)
		
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range(-10, 10)) = 1
    	_CubeMap ("Cube Map", CUBE) = "" {}
		[Toggle] 
		USE_REMAP("Remap Cubemap", Float) = 0
    	_CubeStrength ("Cube Map Brightness", Range(0,10)) = 1
		_MappingOffset("Range Map Offset", Range(-2,2)) = 0
		[Toggle]
		USE_FRESNEL("Fresnel Effect", Float) = 0
		_FresnelPower ("Fresnel Power", Range(0,15)) = 0
		_FresnelCutoff ("Fresnel Cutoff", Range(0,1)) = 0
		[Toggle]
		USE_REFRACTION("Use Refraction", Float) = 1
		_RefractionAmount("Refract Amount", Range(0,1)) = 1
		_Alpha ("Alpha", Range(0,1)) = 1
		[Toggle]
		DO_REFLECT("Reflectino", Float) = 0
		[Toggle]
		WORLD_REFLECT("World Refl", Float) = 0

	}

	SubShader {
		Tags{
			"Queue" = "Transparent" "RenderType" = "Transparent"
		}

		Blend [_SrcBlend] [_DstBlend]
        BlendOp [_BlendOp]
        Cull [_CullMode]

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature ___ USE_FRESNEL_ON
			#pragma shader_feature ___ USE_REMAP_ON
			#pragma shader_feature ___ USE_REMAP_OFF
			#pragma shader_feature ___ DO_REFLECT_ON
			#pragma shader_feature ___ USE_REFRACTION_ON
			#pragma shader_feature ___ WORLD_REFLECT_ON


			#include "UnityCG.cginc"
			#include "HydraShaderTools.cginc"

			float4 _MainColor;
			float4 _SecondaryColor;
			sampler2D _NormalMap;
			float4 _NormalMap_ST;
      		samplerCUBE _CubeMap;

      		float _CubeStrength;
			float _NormalStrength;
			half _UseRemap;
			float _FresnelPower;
			float _FresnelCutoff;
			float _MappingOffset;
			half _Alpha;
			float _RefractionAmount;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 tangent : TANGENT;
			};

			struct v2f {
			
                float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				float3 normal : TEXCOORD3;
				float3 tangent : TEXCOORD4;
				float3 bitangent : TEXCOORD5;
			};


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.uv;
				o.uv.zw = TRANSFORM_TEX(v.uv,_NormalMap);
				o.tangent = UnityObjectToWorldDir(v.tangent);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.bitangent = cross(o.tangent, o.normal);

				o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float3 c;
				float3 normalMap = UnpackNormal(tex2D(_NormalMap, i.uv.zw));
				normalMap *= float3(_NormalStrength, _NormalStrength, 1);
				float3 finalNormal = normalMap.r * i.tangent + normalMap.g * i.bitangent + normalMap.b * i.normal;

				#if USE_REFRACTION_ON
        			c = (texCUBE(_CubeMap, reflect(refract(-i.viewDir,finalNormal, _RefractionAmount), finalNormal)) * _CubeStrength).rgb;
				#else
					c = (texCUBE(_CubeMap, reflect(-i.viewDir, finalNormal)) * _CubeStrength).rgb;
				#endif
				

				#if USE_REMAP_ON
					c = lerp(_MainColor, _SecondaryColor, c.r+_MappingOffset);
				#endif
				#if USE_REMAP_OFF
					c *= _MainColor.rgb;
				#endif

				#if WORLD_REFLECT_ON
					float3 worldView = reflect(-i.viewDir,finalNormal);
					c = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldView);
				#endif

				#if DO_REFLECT_ON
					float3 refl = reflect(-i.viewDir, finalNormal);
					float3 sampleRefl = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, refl);
					c *= 0.5;
					c += sampleRefl*0.5;
				#endif

				#if USE_FRESNEL_ON
					float fres = fresnel(i.viewDir, finalNormal, _FresnelPower, _FresnelCutoff);
					c *= 1-fres;
					float3 coloredFresnel = lerp(_SecondaryColor, _MainColor, fres);
					coloredFresnel *= fres;
					c += coloredFresnel.rgb;
				#endif

				
				return fixed4(c,_Alpha);
			}
		ENDCG
		}
		
	}
	FallBack "Diffuse"
}