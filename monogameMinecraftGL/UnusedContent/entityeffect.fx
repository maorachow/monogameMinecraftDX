﻿matrix World;
matrix View;
matrix Projection;
matrix LightSpaceMat;
matrix LightSpaceMatFar;
float3 viewPos;
 
 
 
texture TextureE;
bool receiveShadow;
float3 DiffuseColor = float3(1, 1, 1);
 
sampler2D textureSampler = sampler_state
{
    Texture = <TextureE>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};
 

sampler ShadowMapSampler = sampler_state
{
    texture = <ShadowMapC>;
    magfilter = Point;
    minfilter = Point;
    mipfilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler ShadowMapSamplerFar = sampler_state
{
    texture = <ShadowMapCFar>;
    magfilter = Point;
    minfilter = Point;
    mipfilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};
struct VertexShaderInput
{
    float4 Position : POSITION0;
  
    float2 TexureCoordinate : TEXCOORD0;
    
};

struct VertexShaderOutput
{
   
    float4 Position : SV_POSITION;
 
    float2 TexureCoordinate : TEXCOORD0;
 
    float4 LightSpacePosition : TEXCOORD2;
    float4 LightSpacePositionFar : TEXCOORD1;
};

struct PixelShaderOutput
{
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
   
    //if(Alpha<1){output.Position.y=0;}
 
 
  
    output.TexureCoordinate = input.TexureCoordinate;
    output.LightSpacePositionFar = mul(worldPosition, LightSpaceMatFar);
    output.LightSpacePosition = mul(worldPosition, LightSpaceMat);
    
    return output;
}
float ShadowCalculation(float4 fragPosLightSpace,sampler2D sp)
{
   
    float3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
   
    projCoords.xy = projCoords * 0.5 + 0.5;
    projCoords.y = 1 - projCoords.y;
    bool isOutBounds = false;
    if (projCoords.x < 0 || projCoords.x > 1 || projCoords.y < 0 || projCoords.y > 1)
    {
        isOutBounds = true;
    }
    float closestDepth = tex2D(sp, projCoords.xy).r;
   
    float currentDepth = projCoords.z;
    float shadow=0;
    float shadowBias = -0.002;
    float2 texelSize = 1.0 / 2048.0;
    for (int x = -1; x <= 1; ++x)
    {
        for (int y = -1; y <= 1; ++y)
        {
            float pcfDepth = tex2D(sp, projCoords.xy + float2(x, y) * texelSize).r;
         //   shadow += currentDepth - shadowBias > pcfDepth ? 1.0 : 0.0;
            if (pcfDepth - shadowBias < currentDepth)
            {
                shadow += 0;
            }
            else
            {
                shadow += 1;
            }
        }
    }
    shadow /= 9.0;
    /*if (closestDepth - shadowBias < currentDepth)
    {
        shadow = 0;
    }
    else
    {
        shadow = 1;
    }*/
    if (closestDepth <= 0.001)
    {
        shadow = 1.0;
    }
 //   float shadow = currentDepth/closestDepth ;
   
    /*if (projCoords.z > 1.0)
    {
        shadow = 1.0;
    }*/
    if (isOutBounds == true)
    {
        shadow = 1.0;
    }
    return shadow;
}
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    
    PixelShaderOutput output = (PixelShaderOutput) 0;

    output.Color = tex2D(textureSampler, input.TexureCoordinate);
   
    output.Color *= float4(DiffuseColor, 1);
 
    
 
 
  
    float shadow = 1;
    
    float shadowFinal = clamp(shadow, 0, 1);
    if (receiveShadow == true)
    {
       output.Color.rgb *= (0.5 + (shadowFinal * 0.5)); 
    }
    else
    {
        output.Color.rgb *= (0.5);
    }
    
 
 
    return output;
}

technique EntityTechnique
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}