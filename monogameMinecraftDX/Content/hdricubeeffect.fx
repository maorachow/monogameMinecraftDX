#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

float4x4 View;
float4x4 Projection;
sampler2D HDRImageSampler = sampler_state
{
    texture = <HDRImageTex>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	 
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float3 LocalPos : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
  
    float4 viewPosition = mul(input.Position, View);
    output.Position = mul(viewPosition, Projection);
 

    output.LocalPos = input.Position.xyz;
	return output;
}
#define invAtan float2(0.1591, 0.3183)
float2 SampleSphericalMap(float3 v)
{
    float2 uv = float2(atan2(v.z, v.x), asin(v.y));
    uv *= invAtan;
    uv += 0.5;
    uv.y = 1 - uv.y;
    return uv;
}
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = SampleSphericalMap(normalize(input.LocalPos)); // make sure to normalize localPos
    
    
    
    float3 color = tex2D(HDRImageSampler, uv).rgb;
    return float4(color.xyz, 1);
}

technique HDRICubemap
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};