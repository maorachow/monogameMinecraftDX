#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

matrix World;
matrix View;
matrix Projection;
matrix LightSpaceMat;
matrix LightSpaceMatFar;

float3 LightPos = float3(20, 70, 30);
float3 LightDir = float3(20, 40, 30);
float3 LightColor = float3(1, 1, 1);
float3 viewPos;
float Alpha;
float fogStart = 256;
float fogRange = 512;

float3 LightPosition1;
float3 LightPosition2;
float3 LightPosition3;
float3 LightPosition4;
float shadowBias;
bool renderShadow;
bool receiveShadow;
//float metallic;
//float roughness;
sampler2D texNoise = sampler_state
{
    Texture = <NoiseTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};
float ShadowCalculation(float4 fragPosLightSpace, sampler2D sp, float bias)
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
     
    float2 texelSize = 1.0 / 2048.0;
     
    for (int x = -1; x <= 1; ++x)
    {
        for (int y = -1; y <= 1; ++y)
        {
            float pcfDepth = tex2D(sp, projCoords.xy + (tex2D(texNoise, projCoords.xy + float2(x, y) * 0.1).rg * 4 - 2)  * texelSize).r;
         //   shadow += currentDepth - shadowBias > pcfDepth ? 1.0 : 0.0;
            if (pcfDepth - shadowBias + bias > currentDepth)
            {
                shadow += 1;
            }
            else
            {
                shadow += 0;
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
    if (closestDepth <= 0.03)
    {
        shadow = 1.0;
    }
 //   float shadow = currentDepth/closestDepth ;
   
    if (projCoords.z > 1.0)
    {
        shadow = 1.0;
    }
    if (isOutBounds == true)
    {
        shadow = 1.0;
    }
    return shadow;
}

float3 CaculatePointLight(float3 lightPos, float3 fragPos, float3 normal, float3 viewDir)
{
    if (abs(lightPos.x) < 0.001 && abs(lightPos.x) < 0.001 && abs(lightPos.x) < 0.001)
    {
        return float3(0, 0, 0);
    }
    float3 lightDir = normalize(lightPos - fragPos);
    
    float diff = max(dot(normal, lightDir), 0.0);
    float diffBack = max(dot(normal, -lightDir), 0) * 0.3;
 
    float3 reflectDir = reflect(-lightDir, normal);
    float3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), 128);
    
    float distance = length(lightPos - fragPos);
    float attenuation = 1.0 / (5 * (distance * distance));
    
    float3 ambient = 0;
    float3 diffuse = diff * float3(255 / 255, 255 / 255, 150.0 / 255.0);
    float3 specular = spec;
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

sampler2D AOSampler = sampler_state
{
    Texture = <TextureAO>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
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
sampler2D AlbedoSampler = sampler_state
{
    Texture = <TextureAlbedo>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
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

sampler2D NormalsSampler = sampler_state
{
    Texture = <TextureNormals>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
sampler2D ContactShadowSampler = sampler_state
{
    Texture = <TextureContactShadow>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Border;
    AddressV = Border;
};
sampler2D reflectionSampler = sampler_state
{
    Texture = <TextureReflection>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler ShadowMapSampler = sampler_state
{
    texture = <ShadowMap>;
    magfilter = Point;
    minfilter = Point;
    mipfilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler ShadowMapFarSampler = sampler_state
{
    texture = <ShadowMapFar>;
    magfilter = Point;
    minfilter = Point;
    mipfilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D gPositionWS = sampler_state
{
    Texture = <PositionWSTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 ProjectionParams2;
float4 CameraViewTopLeftCorner;
float4 CameraViewXExtent;
float4 CameraViewYExtent;

 




float LinearizeDepth(float depth)
{
    float NEAR = 0.1;
    float FAR = 500.0f;
    float d = depth;
    return 1.0 / (d * (1 / FAR - 1 / NEAR) + 1 / NEAR);
}

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

	output.Position =input.Position;
	output.TexCoords = input.TexCoords;

	return output;
}
struct PixelShaderOutput
{
    float4 Color : COLOR0;
    float4 ColorSpecular : COLOR1;
};


const float PI = 3.14159265359;

float DistributionGGX(float3 N, float3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(float3 N, float3 V, float3 L, float roughness);
float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness);



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
float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    return F0 + (max(float3(1.0 - roughness, 1.0 - roughness, 1.0 - roughness), F0) - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}
 
float3 CalculateDirectionalLightP(float3 N, float3 H, float3 V, float3 albedo, float roughness, float3 F0, float3 L)
{
    
    
  
    float3 Lo = float3(0.0, 0.0, 0.0);
    
    
    
    
    
   
    
     
    float attenuation = 1.0 / 1;
    float3 radiance = LightColor * attenuation;
    
    
    float D = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);
    float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
    float3 kS = F;
    float3 kD = float3(1.0, 1.0, 1.0) - kS;
    
    float3 nominator = D * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001;
    float3 specular = nominator / denominator;
    
    float NdotL = max(dot(N, L), 0.0);
    Lo = (kD * albedo / PI + specular) * radiance * NdotL;
    return Lo;
}
float3 CalculateLightP(float3 W, float3 LP, float3 N, float3 V, float3 albedo, float roughness, float3 F0, bool isDirectionalLight = false)
{
    
    if (abs(LP.x) < 0.001 && abs(LP.y) < 0.001 && abs(LP.z) < 0.001)
    {
        return float3(0, 0, 0);
    }
    float3 Lo = float3(0.0, 0.0, 0.0);
    
    
    
    
    float3 L = normalize(LP - W);
    float3 H = normalize(V + L);
   
    
    float distance = length(LP - W);
    float attenuation = 1.0 / (distance * distance);
    if (isDirectionalLight)
    {
        attenuation = 1;
    }
    float3 radiance = LightColor * attenuation;
    
    
    float D = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);
    float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
    float3 kS = F;
    float3 kD = float3(1.0, 1.0, 1.0) - kS;
    
    float3 nominator = D * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001;
    float3 specular = nominator / denominator;
    
    float NdotL = max(dot(N, L), 0.0);
    Lo = (kD * albedo / PI + specular) * radiance * NdotL;
    return Lo;
}


float3 CalculateLightSpecularP(float3 W, float3 LP, float3 N, float3 V, float3 albedo, float roughness, float3 F0, bool isDirectionalLight = false)
{
    
    if (abs(LP.x) < 0.001 && abs(LP.y) < 0.001 && abs(LP.z) < 0.001)
    {
        return float3(0, 0, 0);
    }
    float3 Lo = float3(0.0, 0.0, 0.0);
    
    
    
    
    float3 L = normalize(LP - W);
    float3 H = normalize(V + L);
   
    
    float distance = length(LP - W);
    float attenuation = 1.0 / (distance * distance);
    if (isDirectionalLight)
    {
        attenuation = 1;
    }
    float3 radiance = LightColor * attenuation;
    
    
    float D = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);
    float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
  
    
    float3 nominator = D * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001;
    float3 specular = nominator / denominator;
    
    float NdotL = max(dot(N, L), 0.0);
    Lo = (specular) * radiance * NdotL;
    return Lo;
}


float3 CalculateLightDiffuseP(float3 W, float3 LP, float3 N, float3 V, float3 albedo, float roughness, float3 F0, bool isDirectionalLight,float metallic)
{
    
    if (abs(LP.x) < 0.001 && abs(LP.y) < 0.001 && abs(LP.z) < 0.001)
    {
        return float3(0, 0, 0);
    }
    float3 Lo = float3(0.0, 0.0, 0.0);
    
    
    
    
    float3 L = normalize(LP - W);
    float3 H = normalize(V + L);
   
    
    float distance = length(LP - W);
    float attenuation = 1.0 / (distance * distance);
    if (isDirectionalLight)
    {
        attenuation = 1;
    }
    float3 radiance = LightColor * attenuation;
    
    
   
    float3 F = fresnelSchlickRoughness(max(dot(H, V), 0.0), F0,roughness);
   float3 kS = F;
    float3 kD = float3(1.0, 1.0, 1.0) - kS;
    kD *= 1.0 - metallic;
  
    float NdotL = max(dot(N, L), 0.0);
    Lo = (kD * albedo / PI) * radiance * NdotL;
    return Lo;
}

float3 ReconstructViewPos(float2 uv, float linearEyeDepth)
{
  //  uv.y = 1.0 - uv.y;
    float zScale = linearEyeDepth * ProjectionParams2.x; // divide by near plane  
    float3 viewPos = CameraViewTopLeftCorner.xyz + CameraViewXExtent.xyz * uv.x + CameraViewYExtent.xyz * uv.y;
    viewPos *= zScale;
    return viewPos;
}

float3 LightPositions[4];
PixelShaderOutput MainPS(VertexShaderOutput input)
{
    if (any(tex2D(AlbedoSampler, input.TexCoords).xyz) < 0.01)
    {
        discard;
    }
    PixelShaderOutput output = (PixelShaderOutput) 0;
    
    float3 mer = tex2D(MERSampler, input.TexCoords).xyz;
   // mer.x = 1;
    float3 N = tex2D(NormalsSampler, input.TexCoords).xyz * 2.0 - 1.0;
    float3 worldPos = /*tex2D(gPositionWS, input.TexCoords).xyz;*/ ReconstructViewPos(input.TexCoords,tex2D(DepthSampler, input.TexCoords).r)+viewPos;
  
    float4 LightSpacePosition = mul(float4(worldPos, 1), LightSpaceMat);

    float4 LightSpacePositionFar = mul(float4(worldPos, 1), LightSpaceMatFar);
    float3 camPos = viewPos;
    float3 V = normalize(camPos - worldPos);
    float3 albedo = pow(tex2D(AlbedoSampler, input.TexCoords).rgb, 2.2);
    
    float3 F0 = float3(0.04, 0.04, 0.04);
    F0 = lerp(F0, albedo, mer.x);
     float3 L = normalize(LightDir);
    float3 Lo = float3(0.0, 0.0, 0.0);
    float3 LoDirLight = float3(0.0, 0.0, 0.0);
    float3 H = normalize(V + L);
    
    
    float3 LoSpec = float3(0.0, 0.0, 0.0);
    float3 LoDirLightSpec = float3(0.0, 0.0, 0.0);
    
  /* 
    
    float3 H = normalize(V + L);
    float attenuation = 1.0 /1;
    float3 radiance = LightColor * attenuation;
    
    
    float D = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);
    float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
    float3 kS = F;
    float3 kD = float3(1.0, 1.0, 1.0) - kS;
    
    float3 nominator = D * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001;
    float3 specular = nominator / denominator;
    
    float NdotL = max(dot(N, L), 0.0);
    Lo += (kD * albedo / PI + specular) * radiance * NdotL;*/
    float3 emission = albedo / PI * mer.y*4;
    float roughness = mer.z;
    LoDirLight += CalculateLightDiffuseP(worldPos, worldPos + LightDir, N, V, albedo, roughness, F0, true, mer.x);
    
    for (int i = 0; i < 4; i++)
    {
        Lo += CalculateLightDiffuseP(worldPos, LightPositions[i], N, V, albedo, roughness, F0, false, mer.x);
    }
  /*      Lo += CalculateLightDiffuseP(worldPos, LightPosition1, N, V, albedo, roughness, F0, false, mer.x);
    Lo += CalculateLightDiffuseP(worldPos, LightPosition2, N, V, albedo, roughness, F0, false, mer.x);
    Lo += CalculateLightDiffuseP(worldPos, LightPosition3, N, V, albedo, roughness, F0, false, mer.x);
    Lo += CalculateLightDiffuseP(worldPos, LightPosition4, N, V, albedo, roughness, F0, false, mer.x);*/
   
    LoDirLightSpec += CalculateLightSpecularP(worldPos, worldPos + LightDir, N, V, albedo, roughness, F0, true);
    
    
    for (int j= 0; j < 4; j++)
    {
        LoSpec += CalculateLightSpecularP(worldPos, LightPositions[j], N, V, albedo, roughness, F0, false);
    }
 /*   LoSpec += CalculateLightSpecularP(worldPos, LightPosition1, N, V, albedo, roughness, F0, false);
    LoSpec += CalculateLightSpecularP(worldPos, LightPosition2, N, V, albedo, roughness, F0, false);
    LoSpec += CalculateLightSpecularP(worldPos, LightPosition3, N, V, albedo, roughness, F0, false);
    LoSpec += CalculateLightSpecularP(worldPos, LightPosition4, N, V, albedo, roughness, F0, false);*/
    float3 ambient = float3(0.01, 0.01, 0.01) * albedo * tex2D(AOSampler, input.TexCoords.xy).r;
    
    
    float shadow;
    float shadow1;
    if (receiveShadow == true)
    {
        shadow = ShadowCalculation(LightSpacePosition, ShadowMapSampler, 0);
        float viewZ = -mul(float4(worldPos, 1), View).z;
        if (viewZ > 30)
        {
            shadow1 = ShadowCalculation(LightSpacePositionFar, ShadowMapFarSampler, 0);
        }
        else
        {
            shadow1 = 1;
        }
    }
    
    float3 result;
    float shadowFinal = min(shadow, shadow1);
    shadowFinal = min(shadowFinal, tex2D(ContactShadowSampler, input.TexCoords).x);
    
    float3 color; 
    float3 colorSpec;
    if (receiveShadow == true)
    {
        color =  0+ emission + LoDirLight * shadowFinal + Lo;
        colorSpec = LoDirLightSpec * shadowFinal + LoSpec;
    }
    else
    {
        colorSpec = LoSpec;
        color = 0 + emission + Lo;
    }
    
 //color += tex2D(reflectionSampler, input.TexCoords).rgb;
  //  color = color / (color + float3(1.0, 1.0, 1.0));
  //  color = pow(color, float3(1.0 / 1, 1.0 / 1, 1.0 /1));
   
 
    
    
    output.Color = float4(color, 1.0);
    output.ColorSpecular = float4(colorSpec, 1);
    return output;
}

technique DeferredBlockEffectP
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};