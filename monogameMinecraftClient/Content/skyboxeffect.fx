﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
 

float4x4 World;
float4x4 View;
float4x4 Projection;
float mixValue;
float3 CameraPosition;
float3 lightDir;

 
samplerCUBE SkyBoxSampler = sampler_state
{
    texture = <SkyBoxTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};
samplerCUBE SkyBoxNightSampler = sampler_state
{
    texture = <SkyBoxTextureNight>;
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
    float4 Position : POSITION0;
    float3 TextureCoordinate : TEXCOORD0;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 VertexPosition = mul(input.Position, World);
    output.TextureCoordinate = VertexPosition.xyz - CameraPosition;
 
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    #if OPENGL
    return lerp(texCUBE(SkyBoxSampler,float3( input.TextureCoordinate.x,-input.TextureCoordinate.y,input.TextureCoordinate.z)).xyzw, texCUBE(SkyBoxNightSampler,float3( input.TextureCoordinate.x,-input.TextureCoordinate.y,input.TextureCoordinate.z)).xyzw,mixValue);
    #else
    return lerp(texCUBE(SkyBoxSampler, input.TextureCoordinate.xyz).xyzw, texCUBE(SkyBoxNightSampler, input.TextureCoordinate.xyz).xyzw, mixValue);
    #endif
}
technique Skybox
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}