#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

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
matrix View;
matrix ViewProjection;
 
float metallic;
float roughness;
float GameTime;



sampler2D motionVectorTex = sampler_state
{
    Texture = <MotionVectorTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D prevSSIDTex = sampler_state
{
    Texture = <PrevSSIDTexture>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
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

sampler2D noiseTex = sampler_state
{
    Texture = <NoiseTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};


sampler2D gNormalWS = sampler_state
{
    Texture = <NormalTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D gAlbedo = sampler_state
{
    Texture = <AlbedoTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D gLum = sampler_state
{
    Texture = <LumTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};


float PI = 3.14159265359;

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


float3 CalculateLightP(float3 W, float3 LP, float3 N, float3 V, float3 albedo, float roughness, float3 F0, bool isDirectionalLight, float3 LightColor)
{
    
    
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
float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    return F0 + (max(float3(1.0 - roughness, 1.0 - roughness, 1.0 - roughness), F0) - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}
float3 CalculateLightDiffuseP(float3 W, float3 LP, float3 N, float3 V, float3 albedo, float roughness, float3 F0, bool isDirectionalLight, float3 LightColor)
{
    
    
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
    
    
    
    float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
    float3 kS = F;
    float3 kD = float3(1.0, 1.0, 1.0) - kS;
    
    
    
    float NdotL = max(dot(N, L), 0.0);
    Lo = (kD * albedo / PI ) * radiance * NdotL;
    return Lo;
}


float2 GetScreenCoordFromWorldPos(float3 worldPos)
{
    float4 offset = float4(worldPos, 1.0);
    offset = mul(offset, ViewProjection);
    offset.xyz /= offset.w;
    offset.xy = offset.xy * 0.5 + 0.5;
    offset.y = 1 - offset.y;
    return offset.xy;
}
float GetViewDepthFromWorldPos(float3 worldPos)
{
    float4 marchDepthView = mul(float4(worldPos, 1), View);
       
    marchDepthView.z = marchDepthView.z;
    return -marchDepthView.z;
}


VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = input.Position;
    output.TexCoords = input.TexCoords;

	return output;
}

sampler gProjectionDepth = sampler_state
{
    Texture = (ProjectionDepthTex);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = Point;
};
sampler gProjectionDepthM0 = sampler_state
{
    Texture = (ProjectionDepthTexMip0);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = Point;
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
float4 ProjectionParams2;
float4 CameraViewTopLeftCorner;
float4 CameraViewXExtent;
float4 CameraViewYExtent;

float3 CameraPos;
float mixValue;
float3 ReconstructViewPos(float2 uv, float linearEyeDepth)
{
  //  uv.y = 1.0 - uv.y;
    float zScale = linearEyeDepth * ProjectionParams2.x; // divide by near plane  
    float3 viewPos = CameraViewTopLeftCorner.xyz + CameraViewXExtent.xyz * uv.x + CameraViewYExtent.xyz * uv.y;
    viewPos *= zScale;
    return viewPos;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float linearDepth = 0;
    linearDepth = tex2D(gProjectionDepthM0, input.TexCoords).x;
    if (linearDepth >= 900 || linearDepth <= 0.1)
    {
        discard;
    }
    float3 worldPos = ReconstructViewPos(input.TexCoords, linearDepth) + CameraPos; //tex2D(gPositionWS, input.TexCoords).xyz;
    float3 normal = tex2D(gNormalWS, input.TexCoords).xyz * 2 - 1;
    worldPos = worldPos + normal * 0.1 * length(worldPos - CameraPos) / 150;
    float3 randomVec = float3(tex2D(noiseTex, input.TexCoords * 5).r * 2 - 1 + 0.0001 + GameTime, tex2D(noiseTex, input.TexCoords * 5).g * 2 - 1 + 0.0001 + GameTime, 0);
    float3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    float3 bitangent = cross(normal, tangent);
    float3x3 TBN = float3x3(tangent, bitangent, normal);
    float3 mer = tex2D(MERSampler, input.TexCoords).xyz;
    float3 rayOrigin = worldPos + normalize(normal) * 0.01;
    float3 finalColor = 0;
    float2 prevTexCoord = input.TexCoords+tex2D(motionVectorTex, input.TexCoords).xy;
    float4 prevColor = prevTexCoord.x > 0 && prevTexCoord.y > 0 && prevTexCoord.x < 1 && prevTexCoord.y < 1 ? tex2D(prevSSIDTex, prevTexCoord).xyzw : 0;
    float3 F0 = float3(0.04, 0.04, 0.04);
    float3 albedo = pow(tex2D(gAlbedo, input.TexCoords).xyz, 2.2);
    F0 = lerp(F0, albedo, mer.x);
    float3 N = normal;
    float3 W = worldPos;
    float3 V = normalize(CameraPos - W);
            
    float3 F = fresnelSchlickRoughness(max(dot(N, V), 0.0), F0, mer.z);
    
    float3 kS = F;
    float3 kD = 1.0 - kS;
    kD *= 1.0 - mer.x;
    
    float finalMixVal = 0;
    for (int i = 0; i <2; i++)
    {
        float3 sampleDir = float3(tex2D(noiseTex, input.TexCoords * 5 + float2(i / 10.0, i / 10.0) + GameTime*2.0).r * 2 - 1, tex2D(noiseTex, input.TexCoords * 5 + float2(0.5, 0.5) - float2(i / 10.0, i / 10.0) - GameTime*4.0).g * 2 - 1, tex2D(noiseTex, input.TexCoords * 5 - float2(0.8, 0.8) - float2(i / 10.0, i / 10.0) + GameTime*5.0).b);
        sampleDir.z = clamp(sampleDir.z, 0.02, 1);
        sampleDir = normalize(sampleDir);
        sampleDir = mul(sampleDir, TBN);
        bool isHit = false;
        float2 uv = 0;
        float3 marchPos = 0;
        int mipLevel = 0;
        float strideLen = 1.0;
        float strideNoiseVal = tex2D(noiseTex, input.TexCoords * 5*i + GameTime * 2.5).r - 0.5;
        marchPos = rayOrigin + sampleDir * 0.01;
        float3 preMarchPos = marchPos;
        for (int j = 0; j < 16; j++)
        {
            marchPos += sampleDir * 0.15 * (1+strideNoiseVal) * strideLen;
             uv = GetScreenCoordFromWorldPos(marchPos);
            
            
       //     float3 sampleWorldPos = tex2D(gPositionWS, uv).xyz;
      //      float sampleViewDepth = GetViewDepthFromWorldPos(sampleWorldPos);
            float testDepth = GetViewDepthFromWorldPos(marchPos);
            
            
            float sampleDepthM0 = tex2D(gProjectionDepthM0, uv.xy).x;

        /*    float sampleDepthM1 = tex2D(gProjectionDepthM1, uv.xy).x;
           
            float sampleDepthM2 = tex2D(gProjectionDepthM2, uv.xy).x;

            float sampleDepthM3 = tex2D(gProjectionDepthM3, uv.xy).x;

            float sampleDepthM4 = tex2D(gProjectionDepthM4, uv.xy).x;

            float sampleDepthM5 = tex2D(gProjectionDepthM5, uv.xy).x;
            float sampleDepthM6 = tex2D(gProjectionDepthM6, uv.xy).x;
            float sampleDepthM7 = tex2D(gProjectionDepthM7, uv.xy).x;
            float sampleDepthArray[8] = { sampleDepthM0, sampleDepthM1, sampleDepthM2, sampleDepthM3, sampleDepthM4, sampleDepthM5, sampleDepthM6, sampleDepthM7 };*/ /*GetViewDepthFromWorldPos(worldPosSampled)*/ //tex2D(gProjectionDepth,uv.xy).x;
            float sampleDepth = sampleDepthM0;
            
            if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            {
                isHit = false;
           
                break; // return float4(1, 1, 1, 1);
            }
           
           
                
                
            if (sampleDepth < testDepth && abs(sampleDepth - testDepth) < 0.3 * (1 + strideNoiseVal) * strideLen && mipLevel <= 0)
            {
                uv = GetScreenCoordFromWorldPos(marchPos);
                isHit = true;
           
                break; //   return float4(0, 0, 0, 1);
            }  
            
           
          

        }
        if (isHit == true)
        {
        //    float3 lum = tex2D(gLum, uv).xyz;
          
            float3 lum1 = tex2D(gLum, uv).xyz;
      
            float3 Lo = lum1;
            finalColor += Lo;
            finalMixVal += 1;

        }
        else
        {
            float3 irradiance = lerp(texCUBE(irradianceSampler, normal).rgb, texCUBE(irradianceSamplerNight, normal).rgb, mixValue);
           
            finalColor += 0;
            finalMixVal += 0;
        }
    }      
    finalColor /=2;
    finalMixVal /= 2;
   // finalColor = finalColor*0.01+prevColor;
           
  //      finalColor = finalColor * 0.01 + prevColor;
            float3 irradiance1 = finalColor;
            float3 diffuse = irradiance1 * albedo;
    return lerp(float4(diffuse * kD, finalMixVal), prevColor, 0.9);
}

technique SSID
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};