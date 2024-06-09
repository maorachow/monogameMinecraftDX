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
float3 CameraPos;
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

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 worldPos = tex2D(gPositionWS, input.TexCoords).xyz;
    float3 normal = tex2D(gNormalWS, input.TexCoords).xyz * 2 - 1;
    
    float3 randomVec = float3(tex2D(noiseTex, input.TexCoords * 5).r * 2 - 1 + 0.0001 + GameTime, tex2D(noiseTex, input.TexCoords * 5).g * 2 - 1 + 0.0001 + GameTime, 0);
    float3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    float3 bitangent = cross(normal, tangent);
    float3x3 TBN = float3x3(tangent, bitangent, normal);
    
    float3 rayOrigin = worldPos + normalize(normal) * 0.01;
    float3 finalColor = 0;
    float2 prevTexCoord = input.TexCoords+tex2D(motionVectorTex, input.TexCoords).xy;
    float3 prevColor = prevTexCoord.x > 0 && prevTexCoord.y > 0 && prevTexCoord.x < 1 && prevTexCoord.y < 1 ? tex2D(prevSSIDTex, prevTexCoord).xyz : 0;
    float strideNoiseVal = tex2D(noiseTex, input.TexCoords * 5 + GameTime/2.5).r-0.5;
    for (int i = 0; i <1; i++)
    {
        float3 sampleDir = float3(tex2D(noiseTex, input.TexCoords * 5 + float2(i / 10.0, i / 10.0) + GameTime).r * 2 - 1, tex2D(noiseTex, input.TexCoords * 5 + float2(0.5, 0.5) - float2(i / 10.0, i / 10.0) - GameTime).g * 2 - 1, tex2D(noiseTex, input.TexCoords * 5 - float2(0.8, 0.8) - float2(i / 10.0, i / 10.0) + GameTime).b);
        sampleDir.z = clamp(sampleDir.z, 0.2, 1);
        sampleDir = normalize(sampleDir);
        sampleDir = mul(sampleDir, TBN);
        bool isHit = false;
        float2 uv = 0;
        float3 marchPos = 0;
        for (int j = 0; j < 16; j++)
        {
            marchPos = rayOrigin + sampleDir * 0.2 * (j + 1 + strideNoiseVal);
             uv = GetScreenCoordFromWorldPos(marchPos);
            
            
            float3 sampleWorldPos = tex2D(gPositionWS, uv).xyz;
            float sampleViewDepth = GetViewDepthFromWorldPos(sampleWorldPos);
            float testDepth = GetViewDepthFromWorldPos(marchPos);
            
            
            if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            {
                isHit = false;
           
                break; // return float4(1, 1, 1, 1);
            }
            if (sampleViewDepth < testDepth && abs(sampleViewDepth - testDepth) < 0.3)
            {
                isHit = true;
           
                break; //   return float4(0, 0, 0, 1);
            }
        }
        if (isHit == true)
        {
            float3 lum = tex2D(gLum, uv).xyz;
          
           
      
            float3 Lo = lum;
            finalColor += Lo;

        }
    }      
    finalColor /= 1;
   // finalColor = finalColor*0.01+prevColor;
            float3 F0 = float3(0.04, 0.04, 0.04);
            F0 = lerp(F0, tex2D(gAlbedo, input.TexCoords).xyz, metallic);
              float3 N = normal;
            float3 W = worldPos;
            float3 V = normalize(CameraPos - W);
            
            float3 F = fresnelSchlickRoughness(max(dot(N, V), 0.0), F0, roughness);
    
            float3 kS = F;
            float3 kD = 1.0 - kS;
            kD *= 1.0 - metallic;
  //      finalColor = finalColor * 0.01 + prevColor;
            float3 irradiance = finalColor;
            float3 diffuse = irradiance * pow(tex2D(gAlbedo, input.TexCoords).xyz, 2.2);
            return float4(diffuse.xyz * kD * 0.2 + prevColor*0.8, 1);
}

technique SSID
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};