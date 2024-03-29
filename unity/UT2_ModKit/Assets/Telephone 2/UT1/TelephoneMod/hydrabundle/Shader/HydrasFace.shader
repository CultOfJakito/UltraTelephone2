Shader "Hydra/FX/HydrasFace"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        _SubpixelSize("Pixel Group Size", Range(3,100)) = 1
        _SubpixelScale("Subpixel Scale", Float) = 1
        _SubpixelEdgeSizeHorizontal("Horizontal Pixel Margin", Range(0,0.5)) = 0.05
        _SubpixelEdgeSizeVertical("Vertical Pixel Margin", Range(0,0.5)) = 0.05
        _CellPadding ("Cell Padding", Range(0,0.168)) = 0 
        _ImageBlend ("Image Blend", Range(0,1)) = 0
        [Toggle]
        DIAGONAL ("Use Diagonal", Float) = 0

        _WarpTexture("Warp Texture", 2D) = "bump" {}
        _WarpIntensity ("Warp Intensity", Float) = 1
        _WarpSpeed ("Warp Speed", Float) = 1

        _NoiseScale ("Noise Scale", Float) = 1
        _NoiseAmount ("Noise Amount", Float) = 1
        _NoiseSpeed ("Noise Speed", Float) = 1

        _FresnelAmount ("Fresnel Amount", Float) = 1
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)

        [NoScaleOffset]_FaceMask ("Face Mask Texture", 2D) = "black" {}
        _FaceMaskIntensity ("Face Mask Intensity", Float) = 1
        _FaceColor ("Face Mask Color", Color) = (1,1,1,1)
        [Toggle] _GamerFace("Gamer Face", Float) = 0
        [Toggle] _FaceWobble ("Face Wobble", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        BlendOp Add
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma shader_feature ___ DIAGONAL_ON

            #include "UnityCG.cginc"
            #include "HydraShaderTools.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1) //TEXCOORD1
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _SubpixelEdgeSizeHorizontal;
            float _SubpixelEdgeSizeVertical;
            float _SubpixelSize;
            float _RegionCount;
            float _CellPadding;
            float _SubpixelScale;
            float _ImageBlend;

            sampler2D _WarpTexture;
            float4 _WarpTexture_ST;

            float _WarpIntensity;
            float _WarpSpeed;

            float _NoiseAmount;
            float _NoiseScale;
            float _NoiseSpeed;

            float _FresnelAmount;
            float4 _FresnelColor;

            sampler2D _FaceMask;
            float4 _FaceMask_ST;

            float4 _FaceColor;
            float _FaceMaskIntensity;
            float _FaceWobble;

            float _GamerFace;

            float _Alpha;

            float getvalue(float uv, float edgeSize, float colorIndex)
            {
                float regionSize = (1-(edgeSize*2));
                float partitionSize = (regionSize/(_SubpixelSize));
                
                float startRegion = (edgeSize + (partitionSize*(colorIndex))) + _CellPadding;
                float endRegion = (edgeSize + (partitionSize*(colorIndex+1))) - _CellPadding;

                return (uv > startRegion && uv < endRegion) ? 1 : 0;
            }

            float getcolor(float2 uv, float colorIndex)
            {

                if(uv.x > _SubpixelEdgeSizeHorizontal && uv.x < 1-_SubpixelEdgeSizeHorizontal)
                {
                    if(uv.y > _SubpixelEdgeSizeVertical && uv.y < 1-_SubpixelEdgeSizeVertical)
                    {                            
                        float xValid = getvalue(uv.x, _SubpixelEdgeSizeHorizontal, colorIndex);
                        #if DIAGONAL_ON
                        float yValid = getvalue(uv.y, _SubpixelEdgeSizeVertical, colorIndex);
                        #else
                        float yValid = (uv.y > _SubpixelEdgeSizeVertical + _CellPadding && uv.y < (1-_SubpixelEdgeSizeVertical)-_CellPadding) ? 1 : 0;
                        #endif

                        return (xValid == 1 && yValid == 1) ? 1 : 0;
                    } 
                }

                return 0;
            }

            float4 subpixel (float2 uv)
            {
                uv = frac(uv);
                float red = getcolor(uv, 0);
                float green = getcolor(uv, 1);
                float blue = getcolor(uv, 2);
                float alpha = (red != 0 || green != 0 || blue != 0) ? 1 : 0;
                return float4(red, green, blue, alpha);
            }       

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
                o.uv.zw = (o.uv.xy - 0.5) * _SubpixelScale + 0.5;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                o.normal = v.normal;
                o.viewDir = ObjSpaceViewDir(v.vertex);
                return o;
            }

            //Hello? ,':)

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float3 warpTex = UnpackNormal(tex2D(_WarpTexture, float2(_WarpTexture_ST.x*i.uv.x,_WarpTexture_ST.y*i.uv.y + (_WarpSpeed * _Time.x))));
                warpTex.xy *= _WarpIntensity;
                
                float2 uvSample = ((i.uv.zw-0.5)/_SubpixelScale)+ 0.5;

                fixed4 col = tex2D(_MainTex, uvSample + warpTex.xy);

                if(_NoiseAmount != 0)
                {
                    float noise = Unity_GradientNoise_f(float2(i.uv.x, i.uv.y + (_NoiseSpeed * _Time.x)), _NoiseScale);
                    noise *= _NoiseAmount;
                    col.rgb = lerp(col.rgb, (col.rgb*(1-noise))+noise,_NoiseAmount);
                }
                
                float3 faceColorSample = _FaceColor.rgb;
                if(_GamerFace == 1)
                {
                }
                float2 faceUV = i.uv.xy;

                if(_FaceWobble == 1)
                {
                    float2 uvchange = (faceColorSample.xy*0.5+0.5);
                    faceUV = ((faceUV-0.5)/uvchange)+0.5;
                }
                
                float faceMask = tex2D(_FaceMask, uvSample + (warpTex.xy*0.25)).r * _FaceMaskIntensity;

                //col.rgb *= 1-faceMask;
                //col.rgb += faceMask;

                float fres = fresnel(i.viewDir, i.normal, _FresnelAmount);

                fixed4 colorCache = col;
                colorCache.rgb *= 1-fres;
                colorCache.rgb += (1-col.rgb) * fres;



                fixed4 output = float4(colorCache.rgb * subpixel(i.uv.zw).rgb,1);
                //fixed4 output = col;
                

                output.rgb = lerp(output.rgb, colorCache.rgb, _ImageBlend);
                
                output.rgb *= 1-faceMask;
                output.rgb += faceMask*faceColorSample;
                

                //float4 finalOutput = output;
                //finalOutput.rgb *= 1-fres;
                //finalOutput.rgb += fres*(1-output.rgb);
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return output;
            }
            ENDCG
        }
    }
}
