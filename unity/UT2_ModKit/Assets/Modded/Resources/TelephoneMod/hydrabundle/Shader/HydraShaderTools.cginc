#ifndef HYDRASHADERTOOL
#define HYDRASHADERTOOL

inline float invLerp(float from, float to, float value) {
    return (value - from) / (to - from);
}

inline float4 invLerp(float4 from, float4 to, float4 value) {
    return (value - from) / (to - from);
}

inline float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
    float rel = invLerp(origFrom, origTo, value);
    return lerp(targetFrom, targetTo, rel);
}

inline float4 remap(float4 origFrom, float4 origTo, float4 targetFrom, float4 targetTo, float4 value){
    float4 rel = invLerp(origFrom, origTo, value);
    return lerp(targetFrom, targetTo, rel);
}

inline float3 hueShift(float3 color, float hue)
{
	const float3 k = float3(0.57735, 0.57735, 0.57735);
	float cosAngle = cos(hue);
	return float3(color * cosAngle + cross(k, color) * sin(hue) + k * dot(k, color) * (1.0 - cosAngle));
}

//Generates a fresnel effect
inline float fresnel (float3 viewDir, float3 normal)
{
    return 1-saturate(dot(normalize(viewDir), normal));
}

//Generates a fresnel effect and scales with power
inline float fresnel (float3 viewDir, float3 normal, float power)
{
    return pow(fresnel(viewDir, normal),power);
}

//Generates a fresnel effect and scales with power and cuts it off.
inline float fresnel (float3 viewDir, float3 normal, float power, float cutoff)
{
    float fres = fresnel(viewDir, normal, power);
    fres = (fres < cutoff) ? 0 : fres;
    return fres;
}

/*
inline float fresnel (float3 viewDir, float3 normal, float power, float cutoff)
{
    float fres = 1-saturate(dot(normalize(viewDir), normal));
    fres = pow(fres, power);
    fres = (fres < cutoff) ? 0 : fres;
    return fres;
}
*/
//returns a time from sine input mapped between 0 and 1
inline float nsin(float num)
{
    return (0.5*sin(num)+1);
}

//Renders a procedural bar direction 0 Horizontal (X), 1 vertical(Y), 2 Lateral (Z)
float proceduralBar (float direction, float3 vertexPosition, float2 uv, float offset, float scale)
{
    float vertPos;
    float uvPos;
    
    if(direction == 0) //Horizontal
    {
        vertPos = vertexPosition.x;
        uvPos = distance(0.5, uv.y);
        
    }
    else if (direction == 1) //Vertical
    {
        vertPos = vertexPosition.y;
        uvPos = distance(0.5, uv.x);
    }
    else if (direction == 2) //Lateral
    {
        vertPos = vertexPosition.z;
        uvPos = distance(0.5, uv.x);
    }

    offset *= _Time.x; //apply scroll
    return nsin((offset+vertPos) * scale) - uvPos;
}

//Unity URP Funcs from https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/
float4 Unity_Dither_float4 (float4 In, float4 ScreenPosition)
{
    float4 o;

    float2 uv = ScreenPosition.xy * _ScreenParams.xy;
    float DITHER_THRESHOLDS[16] =
    {
        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
    };
    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
    o = In - DITHER_THRESHOLDS[index];
}

inline float2 unity_voronoi_noise_randomVector (float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
}

void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float t = 8.0;
    float3 res = float3(8.0, 0.0, 0.0);

    for(int y=-1; y<=1; y++)
    {
        for(int x=-1; x<=1; x++)
        {
            float2 lattice = float2(x,y);
            float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
            float d = distance(lattice + offset, f);
            if(d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                Out = res.x;
                Cells = res.y;
            }
        }
    }
}

float2 unity_gradientNoise_dir(float2 p)
{
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float unity_gradientNoise(float2 p)
{
    float2 ip = floor(p);
    float2 fp = frac(p);
    float d00 = dot(unity_gradientNoise_dir(ip), fp);
    float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
    float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
    float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
{
    Out = unity_gradientNoise(UV * Scale) + 0.5;
}

float Unity_GradientNoise_f(float2 UV, float Scale)
{
    return unity_gradientNoise(UV * Scale) + 0.5;
}

/*

float3 Unity_NormalFromHeight_Tangent(float In)
{
    float3 worldDirivativeX = ddx(Position * 100);
    float3 worldDirivativeY = ddy(Position * 100);
    float3 crossX = cross(TangentMatrix[2].xyz, worldDirivativeX);
    float3 crossY = cross(TangentMatrix[2].xyz, worldDirivativeY);
    float3 d = abs(dot(crossY, worldDirivativeX));
    float3 inToNormal = ((((In + ddx(In)) - In) * crossY) + (((In + ddy(In)) - In) * crossX)) * sign(d);
    inToNormal.y *= -1.0;
    float3 o = normalize((d * TangentMatrix[2].xyz) - inToNormal);
    return TransformWorldToTangent(o, TangentMatrix);
}

float3 Unity_NormalFromHeight_World(float In)
{
    float3 worldDirivativeX = ddx(Position * 100);
    float3 worldDirivativeY = ddy(Position * 100);
    float3 crossX = cross(TangentMatrix[2].xyz, worldDirivativeX);
    float3 crossY = cross(TangentMatrix[2].xyz, worldDirivativeY);
    float3 d = abs(dot(crossY, worldDirivativeX));
    float3 inToNormal = ((((In + ddx(In)) - In) * crossY) + (((In + ddy(In)) - In) * crossX)) * sign(d);
    inToNormal.y *= -1.0;
    return normalize((d * TangentMatrix[2].xyz) - inToNormal);
}

*/

float Unity_Rectangle_float(float2 UV, float Width, float Height)
{
    float2 d = abs(UV * 2 - 1) - float2(Width, Height);
    d = 1 - d / fwidth(d);
    return saturate(min(d.x, d.y));
}

float Unity_Polygon_float(float2 UV, float Sides, float Width, float Height)
{
    float pi = 3.14159265359;
    float aWidth = Width * cos(pi / Sides);
    float aHeight = Height * cos(pi / Sides);
    float2 uv = (UV * 2 - 1) / float2(aWidth, aHeight);
    uv.y *= -1;
    float pCoord = atan2(uv.x, uv.y);
    float r = 2 * pi / Sides;
    float distance = cos(floor(0.5 + pCoord / r) * r - pCoord) * length(uv);
    return saturate((1 - distance) / fwidth(distance));
}

float4 Unity_Ellipse_float(float2 UV, float Width, float Height)
{
    float d = length((UV * 2 - 1) / float2(Width, Height));
    return saturate((1 - d) / fwidth(d));
}

#endif
