#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

 
sampler2D deferredLumSampler = sampler_state
{
    Texture = <TextureDeferredLum>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
sampler2D albedoSampler = sampler_state
{
    Texture = <TextureAlbedo>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
sampler2D reflectionSampler = sampler_state
{
    Texture = <TextureReflection>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D ssidSampler = sampler_state
{
    Texture = <TextureIndirectDiffuse>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D aoSampler = sampler_state
{
    Texture = <TextureAO>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
samplerCUBE irradianceSampler = sampler_state
{
    texture = <HDRIrradianceTex>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};
samplerCUBE irradianceSamplerNight = sampler_state
{
    texture = <HDRIrradianceTexNight>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};
samplerCUBE preFilteredSpecularSampler = sampler_state
{
    texture = <HDRPrefilteredTex>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};


samplerCUBE preFilteredSpecularSamplerNight = sampler_state
{
    texture = <HDRPrefilteredTexNight>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};


float mixValue;
sampler2D gNormals = sampler_state
{
    Texture = <TextureNormals>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D DepthSampler = sampler_state
{
    Texture = <TextureDepth>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
sampler2D AlbedoSampler = sampler_state
{
    Texture = <TextureAlbedo>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
sampler2D MERSampler = sampler_state
{
    Texture = <TextureMER>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Border;
    AddressV = Border;
};
sampler2D texBRDFLUT = sampler_state
{
    Texture = <LUTTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D deferredLumSpecSampler = sampler_state
{
    Texture = <TextureDeferredLumSpec>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};


sampler2D deferredLumTransSampler = sampler_state
{
    Texture = <TextureDeferredLumTrans>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};

sampler2D deferredLumSpecTransSampler = sampler_state
{
    Texture = <TextureDeferredLumSpecTrans>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
float3 viewPos;
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 TexCoords : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = input.Position;
    output.TexCoords = input.TexCoords;

	return output;
}
float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    return F0 + (max(float3(1.0 - roughness, 1.0 - roughness, 1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}

float4 ProjectionParams2;
float4 CameraViewTopLeftCorner;
float4 CameraViewXExtent;
float4 CameraViewYExtent;
float3 ReconstructViewPos(float2 uv, float linearEyeDepth)
{
  //  uv.y = 1.0 - uv.y;
    float zScale = linearEyeDepth * ProjectionParams2.x; // divide by near plane  
    float3 viewPos = CameraViewTopLeftCorner.xyz + CameraViewXExtent.xyz * uv.x + CameraViewYExtent.xyz * uv.y;
    viewPos *= zScale;
    return viewPos;
}
#define PI 3.14159265359
float DistributionGGX(float3 N, float3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float num = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}
float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

float3 fresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}


 
float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    if (tex2D(albedoSampler, input.TexCoords).a < 0.1 && tex2D(deferredLumTransSampler, input.TexCoords).a<0.01)
    {
        discard;
        
    }
    float3 mer = tex2D(MERSampler, input.TexCoords).xyz;
 //   mer.x = 1;
    float3 albedo = pow(tex2D(AlbedoSampler, input.TexCoords).rgb, 2.2);
    float3 normal = tex2D(gNormals, input.TexCoords).xyz * 2 - 1;
  
    float3 color = tex2D(deferredLumSampler, input.TexCoords).xyz + tex2D(deferredLumSpecSampler, input.TexCoords).xyz;
    float3 reflection = tex2D(reflectionSampler, input.TexCoords).xyz;
    float reflectionAlpha = tex2D(reflectionSampler, input.TexCoords).w;
    
    float3 worldPos = ReconstructViewPos(input.TexCoords, tex2D(DepthSampler, input.TexCoords).r) + viewPos;
    float3 V = normalize(viewPos - worldPos);
    float3 R = reflect(-V, normal);
    R = normalize(R);
    float3 F0 = float3(0.04,0.04,0.04);
    F0 = lerp(F0, albedo, mer.x);
    float3 F = fresnelSchlickRoughness(max(dot(normal, V), 0.0), F0, mer.z);
    float3 kS = F;
    float3 kD = 1.0 - kS;
    kD *= 1.0 - mer.x;
    
  /*  float3 indirectDiffuse = tex2D(ssidSampler, input.TexCoords).xyz;
   
    float3 irradiance = lerp(texCUBE(irradianceSampler, normal).rgb, texCUBE(irradianceSamplerNight, normal).rgb, mixValue);
    float3 diffuse = irradiance * albedo;
    float3 ambientEnv = (kD * diffuse);

    
    indirectDiffuse = lerp(ambientEnv, indirectDiffuse, tex2D(ssidSampler, input.TexCoords).a);*/

     
   /* const float MAX_REFLECTION_LOD = 4.0;
    float3 prefilteredColor = lerp(texCUBElod(preFilteredSpecularSampler, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, texCUBElod(preFilteredSpecularSamplerNight, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, mixValue);
    
    float3 specularEnv = prefilteredColor * (F * brdf.x + brdf.y) * 0.1;*/
    float2 brdf = tex2D(texBRDFLUT, float2(max(dot(normal, V), 0.0), 1 - mer.z)).rg;
    const float MAX_REFLECTION_LOD = 4.0;
    float3 prefilteredColor = lerp(texCUBElod(preFilteredSpecularSampler, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, texCUBElod(preFilteredSpecularSamplerNight, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, mixValue);
         //   float2 brdf = tex2D(texBRDFLUT, float2(max(dot(normal, V), 0.0), 1 - mer.z)).rg;
    float3 specularEnv = prefilteredColor * (F * brdf.x + brdf.y);
    
    float3 ambient = (lerp(specularEnv,reflection, reflectionAlpha) /** 0.5 + reflection*/) * tex2D(aoSampler, input.TexCoords).x;
    float3 final = color + ambient ;
   
    float4 finalTrans = float4(tex2D(deferredLumSpecTransSampler, input.TexCoords).xyz + tex2D(deferredLumTransSampler, input.TexCoords).xyz, tex2D(deferredLumTransSampler, input.TexCoords).w+ tex2D(deferredLumSpecTransSampler, input.TexCoords).w);
    finalTrans.xyz = finalTrans.xyz / (finalTrans.xyz + float3(1.0, 1.0, 1.0));
    finalTrans.xyz = pow(finalTrans.xyz, float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));
    final = final / (final + float3(1.0, 1.0, 1.0));
    final = pow(final, float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));
    final = lerp(final, finalTrans.xyz, finalTrans.w);
    return float4(final.xyz, tex2D(albedoSampler, input.TexCoords).a > 0.0001 ? 1 : finalTrans.w);
}

technique DeferredBlend
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};