Shader "Hydra/WavedRippleIncreasing"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainColor ("Main Color", Color) = (1,1,1,1)
		_CubeMap("Cube Map", CUBE) = "" {}
		[Space(5)]
		[Header(Ripple Properties)]
		[Space(10)]

		_RippleEffectA("Ripple Effect", Range(0,1)) = 1
		_RippleEffectB("Ripple Effect", Range(0,1)) = 1
		_TimeSpeed("Ripple Speed", Float) = 1
		_Amp("Ripple Height", Float) = 1
		_Freq("Ripple Size", Float) = 1
		_RippleRange("Ripple Scale", Range(0.01,500)) = 20
		[Toggle] _DistanceIntensity("Increase Ripple Height By Distance", Float) = 1

		[Space(10)]
		_RippleOrigin ("Ripple Origin", Vector) = (0,0,0,0)
		_RippleOrigin2 ("Ripple Origin 2", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry"}

			CGPROGRAM
			#pragma surface surf Lambert vertex:vert

			struct Input {
				float2 uv_MainTex;
				float3 vertColor;
				float3 viewDir;
				float3 worldRefl; INTERNAL_DATA
			};

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal: NORMAL;
				float4 texcoord: TEXCOORD0;
			};

			sampler2D _MainTex;
			samplerCUBE _CubeMap;
			float4 _MainColor;
			float _TimeSpeed;
			float _Freq;
			fixed _RippleEffectA;
			fixed _RippleEffectB;
			float _Amp;
			float _MaxAmp;
			float4 _RippleOrigin;
			float4 _RippleOrigin2;
			float _RippleStep;
			float _RippleRange;
			half _DistanceIntensity;

			
			void vert (inout appdata v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				float t = _Time * _TimeSpeed;
                float dist = distance(_RippleOrigin, v.vertex.xyz);
                float dist2 = distance(_RippleOrigin2, v.vertex.xyz);
                //float offset = sin((dist*_Freq) + t) * _Amp;
                //float offset = sin((_Time.x * _Freq * _TimeSpeed) + dist*_Freq) * clamp(0.0,_MaxAmp,(_Amp * (dist/_RippleRange)));
				float ampDist = (_DistanceIntensity == 1) ? (dist/_RippleRange) : 1;
				float ampDist2 = (_DistanceIntensity == 1) ? (dist2/_RippleRange) : 1;
                float offset1 = sin((_Time.x * _Freq * -_TimeSpeed) + dist*_Freq) * (_Amp * ampDist) * _RippleEffectA;
                float offset2 = sin((_Time.x * _Freq * -_TimeSpeed) + dist2*_Freq) * (_Amp * ampDist2) * _RippleEffectB;
				float offset = max(offset1,offset2);
                //v.vertex.y += (offset*_RippleStep);
                v.vertex.y += (offset);
				//v.normal = normalize(float3(v.normal.x+(offset*_RippleStep), v.normal.y, v.normal.z+(offset*_RippleStep)));
				v.normal = normalize(float3(v.normal.x+(offset), v.normal.y, v.normal.z+(offset)));
				//v.normal = normalize(v.normal.xyz + waveHeight);
				o.vertColor = 1;
			}

			void surf (Input IN, inout SurfaceOutput o){
				float4 c = tex2D(_MainTex, IN.uv_MainTex);
				//c *= texCUBE(_CubeMap, WorldReflectionVector(IN, o.Normal));
				o.Albedo = c.rgb;
			}

			ENDCG
		}
	
}
