#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif
 
/*sampler gProjectionDepth = sampler_state
{
    Texture = (ProjectionDepthTex);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = NONE;
};*/
Texture2D PositionWSTex;
Texture2D NormalTex;
Texture2D LumTex;
//Texture2D RoughnessMap;
 
float GameTime;
float metallic;
float roughness;
SamplerState defaultSampler
{
 //   Texture = (PositionWSTex);
  
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Border;
    AddressV = Border;
};

sampler2D motionVectorTex = sampler_state
{
    Texture = <MotionVectorTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D prevSSRTex = sampler_state
{
    Texture = <PrevSSRTexture>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
 
sampler gNormal = sampler_state
{
    Texture = (NormalTex);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = NONE;
};
sampler gAlbedo = sampler_state
{
    Texture = (AlbedoTex);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = NONE;
};
sampler gRoughness = sampler_state
{
    Texture = (RoughnessMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = NONE;
};


sampler2D texNoise = sampler_state
{
    Texture = <NoiseTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
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

float4x4 matInverseView;

float4x4 matTransposeView;
float4x4 matInverseProjection;
float4x4 matView;
float4x4 matProjection;
 
matrix ViewProjection;
matrix View;
matrix Projection;
matrix ViewOrigin;
bool binarySearch;
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position =input.Position;
    output.TexCoord = input.TexCoord;

	return output;
}
/*float GetDepth(float2 texCoord)
{
    return tex2D(gProjectionDepthM0, texCoord).r;
}*/

float4 ProjectionParams2;
float4 CameraViewTopLeftCorner;
float4 CameraViewXExtent;
float4 CameraViewYExtent;

float3 CameraPos;
float3 ReconstructViewPos(float2 uv, float linearEyeDepth)
{
  //  uv.y = 1.0 - uv.y;
    float zScale = linearEyeDepth * ProjectionParams2.x; // divide by near plane  
    float3 viewPos = CameraViewTopLeftCorner.xyz + CameraViewXExtent.xyz * uv.x + CameraViewYExtent.xyz * uv.y;
    viewPos *= zScale;
    return viewPos;
}
float3 GetUVFromPosition(float3 worldPos)
{
    float4 viewPos = mul(float4(worldPos, 1), matView);
    float4 projectionPos = mul(viewPos, matProjection);
    projectionPos.xyz /= projectionPos.w;
    projectionPos.y = -projectionPos.y;
    projectionPos.xy = projectionPos.xy * 0.5 + 0.5;
    return projectionPos.xyz;
}

float3 GetWorldPosition(float2 vTexCoord, float depth)
{
    // Get the depth value for this pixel
    float z = depth;
    // Get x/w and y/w from the viewport position
    float x = vTexCoord.x * 2 - 1;
    float y = (1 - vTexCoord.y) * 2 - 1;
    float4 vProjectedPos = float4(x, y, z, 1.0f);
    // Transform by the inverse projection matrix
    float4 vPositionVS = mul(vProjectedPos, matInverseProjection);
    float4 vPositionWS = mul(vPositionVS, matInverseView);
    // Divide by w to get the view-space position
    return vPositionWS.xyz / vPositionWS.w;
}
/*RayTraceOutput TraceRay(float2 TexCoord)
{
    
    float InitDepth = GetDepth(TexCoord);
    if (InitDepth <= 0.00001)
    {
        return (RayTraceOutput) 0;
    }
// Now get the position
    float3 reflPosition = GetWorldPosition(TexCoord, InitDepth);
// Get the Normal Data
    float3 normalData = tex2D(gNormal,TexCoord).xyz;
//tranform normal back into [-1,1] range
    float3 reflNormal = 2.0f * normalData - 1.0f;
    float3 vDir = normalize(reflPosition - CameraPos);
    
    float diff = max(dot(reflNormal, vDir), -1.0);
    if (diff <=- 0.7)
    {
        return (RayTraceOutput) 0;
    }
    
    float3 reflectDir = normalize(reflect(vDir, normalize(reflNormal)));
    RayTraceOutput output = (RayTraceOutput) 0;
    float3 curPos = reflPosition;
 
    // The Current UV
    float3 curUV = 0;
 
    // The Current Length
    float curLength = 0.5;

    // Now loop
     
    for (int i = 0;i <64; i++)
    {
        // Has it hit anything yet
        if (output.Hit == false)
        {
            // Update the Current Position of the Ray
            curPos = reflPosition+reflectDir * curLength;
            // Get the UV Coordinates of the current Ray
            curUV = GetUVFromPosition(curPos);
            // The Depth of the Current Pixel
            float curDepth = GetDepth(curUV.xy);
             
                if (abs(curUV .z - curDepth) < 0.00001)
                {
                    // If it's hit something, then return the UV position
                    output.Hit = true;
                    output.UV = curUV .xy;
                    break;
                }
            if (curDepth < 0.00001||curUV.z<0.00001)
            {
                    // If it's hit something, then return the UV position
                output.Hit = false;
                output.UV = curUV.xy;
                break;
            }
               // curDepth = GetDepth(curUV.xy + (float2(0.01, 0.01) * 2));
           

            // Get the New Position and Vector
            float3 newPos = GetWorldPosition(curUV.xy, curDepth);
            curLength = length(reflPosition - newPos);
        }
    }
    return output;
}
float4 MainPS(VertexShaderOutput input) : COLOR
{
 
   //return float4((input.TexCoord).xy, 1, 1);
    RayTraceOutput ray = TraceRay(input.TexCoord);
    float amount = 0.1;
    if (ray.Hit == true)
    {
                        // Fade at edges
        if (ray.UV.x < 0 || ray.UV.x > 1 || ray.UV.y < 0 || ray.UV.y > 1  )
        {
            return float4(0, 0, 0, 0);
        }
        return float4(tex2D(gAlbedo, ray.UV.xy).xyz * tex2D(gRoughness, input.TexCoord.xy).r*0.4, 0.1);
    }
    else
    {
        return float4(0,0,0, 0);

    }
    
   
}*/

float2 GetScreenCoordFromWorldPos(float3 worldPos)
{
    float4 offset = float4(worldPos, 1.0);
    offset = mul(offset, ViewProjection);
    offset.xyz /= offset.w;
    offset.xy = offset.xy * 0.5 + 0.5 + (offset.z * 0.0000000001);
    offset.y = 1 - offset.y;
    return offset.xy;
}
float GetViewDepthFromWorldPos(float3 worldPos)
{
    float4 marchDepthView = mul(float4(worldPos, 1), View);
       
    marchDepthView.z = marchDepthView.z + marchDepthView.x * 0.00001 + marchDepthView.y * 0.000001;
    return -marchDepthView.z;
}
float4 TransformViewToHScreen(float3 vpos, float2 screenSize)
{
    float4 cpos = mul( float4(vpos, 1), Projection);
    cpos.xy = float2(cpos.x, -cpos.y) * 0.5 + 0.5 * cpos.w;//
    cpos.xy *= screenSize;
    return cpos;
}


float Random2DTo1D(float2 value, float a, float2 b)
{
	            //avaoid artifacts
    float2 smallValue = sin(value);
	            //get scalar value from 2d vector	
    float random = dot(smallValue, b);
    random = frac(sin(random) * a);
    return random;
}
float Random2DTo1D(float2 value)
{
    return (
		            Random2DTo1D(value, 14375.5964, float2(15.637, 76.243))
		          
	            );
}
void swap(inout float v0, inout float v1)
{
    float temp = v0;
    v0 = v1;
    v1 = temp;
}
 float PI = 3.14159265359;

float DistributionGGX(float3 N, float3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(float3 N, float3 V, float3 L, float roughness);
float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness);


float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    return F0 + (max(float3(1.0 - roughness, 1.0 - roughness, 1.0 - roughness), F0) - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}
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


float3 CalculateLightP(float3 W, float3 LP, float3 N, float3 V, float3 albedo, float roughness, float3 F0, bool isDirectionalLight,float3 LightColor)
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



float3 CalculateLightSpecularP(float3 W, float3 LP, float3 N, float3 V, float3 albedo, float roughness, float3 F0, bool isDirectionalLight, float3 LightColor)
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
 //   D = clamp(D, 0, 1);
    float G = GeometrySmith(N, V, L, roughness);
    float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
    float3 nominator = D * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001;
    float3 specular = nominator / denominator;
    
    float NdotL = max(dot(N, L), 0.0);
    Lo = (specular) * radiance * NdotL;
    return Lo;
}



float3 CalculateLightDiffuseP(float3 W, float3 LP, float3 N, float3 V, float3 albedo, float roughness, float3 F0, bool isDirectionalLight , float3 LightColor)
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
    
    
   
    float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
    float3 kS = F;
    float3 kD = float3(1.0, 1.0, 1.0) - kS;
    
  
    float NdotL = max(dot(N, L), 0.0);
    Lo = (kD * albedo / PI) * radiance * NdotL;
    return Lo;
}

sampler gProjectionDepthM0 = sampler_state
{
    Texture = (ProjectionDepthTexMip0);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
};

sampler gProjectionDepthM1 = sampler_state
{
    Texture = (ProjectionDepthTexMip1);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
};
sampler gProjectionDepthM2 = sampler_state
{
    Texture = (ProjectionDepthTexMip2);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
};
sampler gProjectionDepthM3 = sampler_state
{
    Texture = (ProjectionDepthTexMip3);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
};
sampler gProjectionDepthM4 = sampler_state
{
    Texture = (ProjectionDepthTexMip4);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
};
sampler gProjectionDepthM5 = sampler_state
{
    Texture = (ProjectionDepthTexMip5);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
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
float2 PixelSize;
float mixValue;

float RadicalInverse_VdC(uint bits)
{
    bits = (bits << 16u) | (bits >> 16u);
    bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
    bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
    bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
    bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
    return float(bits) * 2.3283064365386963e-10; // / 0x100000000
}
// ----------------------------------------------------------------------------
float2 Hammersley(uint i, uint N)
{
    return float2(float(i) / float(N), RadicalInverse_VdC(i));
}
float3 ImportanceSampleGGX(float2 Xi, float3 N, float roughness)
{
    float a = roughness * roughness;
	
    float phi = 2.0 * PI * Xi.x;
    float cosTheta = sqrt((1.0 - Xi.y) / (1.0 + (a * a - 1.0) * Xi.y));
    float sinTheta = sqrt(1.0 - cosTheta * cosTheta);
	
	// from spherical coordinates to cartesian coordinates - halfway vector
    float3 H;
    H.x = cos(phi) * sinTheta;
    H.y = sin(phi) * sinTheta;
    H.z = cosTheta;
	
	// from tangent-space H vector to world-space sample vector
    float3 up = abs(N.z) < 0.999 ? float3(0.0, 0.0, 1.0) : float3(1.0, 0.0, 0.0);
    float3 tangent = normalize(cross(up, N));
    float3 bitangent = cross(N, tangent);
	
    float3 sampleVec = tangent * H.x + bitangent * H.y + N * H.z;
    return normalize(sampleVec);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    float linearDepth = 0;
    linearDepth = tex2D(gProjectionDepthM0, input.TexCoord);
    if (linearDepth >= 900||linearDepth<=0.1)
    {
        discard;
    }
    
    
    float3 worldPos = ReconstructViewPos(input.TexCoord, linearDepth) + CameraPos; // PositionWSTex.Sample(defaultSampler, input.TexCoord).xyz;
    
  //  return float4(worldPos.xyz, 1);
    float3 normal = NormalTex.Sample(defaultSampler, input.TexCoord).xyz * 2 - 1;
    float3 mer = tex2D(MERSampler, input.TexCoord).xyz;
    
    
    
    

  //  float noiseValue = worldPos.x;
    worldPos = worldPos + normal * 0.3 * length(worldPos - CameraPos) / 100;
    
    
    float3 vDir = normalize(worldPos-CameraPos);
    float3 rDir = reflect(vDir, (normal));
    
    float3 rayOrigin = worldPos ;
    float NdotL =2- max(dot(normal, -vDir), 0.0)*1.5;
    NdotL = pow(NdotL,2);
     
    float rayLengthAmp = 1 + NdotL;
    float ssrThickness = 0.2;
   
    bool isRayReturning = false;

   
  //  return float4(noiseValue1.xxx, 1);
   
    
    
    float2 prevTexCoord = input.TexCoord + tex2D(motionVectorTex, input.TexCoord).xy;
    float maxBlendDistance = length(PixelSize) * 2;
    float deltaLength = length(prevTexCoord - input.TexCoord);
    float blendFactor = clamp(deltaLength / maxBlendDistance,0,1);
    float3 prevColor = prevTexCoord.x > 0 && prevTexCoord.y > 0 && prevTexCoord.x < 1 && prevTexCoord.y < 1 ? tex2D(prevSSRTex, prevTexCoord).xyz : 0;
    
    
    
 
    //float2 brdf1 = tex2D(texBRDFLUT, input.TexCoord.xy).rg;
   // return float4(brdf1, 0, 1);
    
  
   
             

            
    float3 N = normal;
    float3 V = normalize(CameraPos - worldPos);
    float3 F0 = float3(0.04, 0.04, 0.04);
    F0 = lerp(F0, pow(tex2D(gAlbedo, input.TexCoord).xyz, 2.2), mer.x);
    float3 F = fresnelSchlickRoughness(max(dot(N, V), 0.0), F0, mer.z);
       //   Lo = Lo / (Lo + float3(1.0, 1.0, 1.0));
       //    Lo = pow(Lo, float3(1.0 / 1, 1.0 / 1, 1.0 / 1));
            
    
    
    
    float2 brdf = tex2D(texBRDFLUT, float2(max(dot(N, V), 0.0), 1 - mer.z)).rg;
    
    float3 finalColor = 0;
    if (mer.z > 0.8f)
    {
        float3 R = reflect(-V, normal);
        R = normalize(R);
        const float MAX_REFLECTION_LOD = 4.0;
        float3 prefilteredColor = lerp(texCUBElod(preFilteredSpecularSampler, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, texCUBElod(preFilteredSpecularSamplerNight, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, mixValue);
       
        float3 specularEnv = prefilteredColor * (F * brdf.x + brdf.y);
    
      //  finalColor += specularEnv;
     
   
        return float4(specularEnv.xyz, 1);
        
        
    }
  
    
 
    
     
        
    
        float3 curRDir = rDir;
        float noiseValue1 = tex2D(texNoise, input.TexCoord * 4.0 + GameTime * 5).g + 0.5;
    
    
    
    
    
     
        
     
     
     //   float3 noiseValue4 = tex2D(texNoise, input.TexCoord * 6.0*(0+1) + GameTime * 9).rgb * 2 - 1;
    
     /*   float3 randomVec = float3(noiseValue2.rg, 0);
        float3 tangent = normalize(randomVec - curRDir * dot(randomVec, curRDir));
        float3 bitangent = cross(curRDir, tangent);
        float3x3 TBN = float3x3(tangent, bitangent, curRDir);
        float3 noiseValue3 = float3(noiseValue4.rg * mer.z * 0.1, noiseValue2.b * 0.5 + 0.5);
        float3 rayRoughnessAmp = mul(noiseValue3, TBN);
    
     
        curRDir = rayRoughnessAmp;
        curRDir = normalize(curRDir);*/
        float2 noiseValue2 = tex2D(texNoise, input.TexCoord * 5.0 * (0 + 1 + 1) + GameTime * 8 * (0 + 1)).rg;
        float3 importanceSampleDir = ImportanceSampleGGX(noiseValue2.xy, rDir, mer.z);
    
    
        curRDir = importanceSampleDir;
        float3 marchPos = worldPos + curRDir * 0.01;
        float3 preMarchPos = rayOrigin;
        int miplevel = 0;
        float strideLen = 1;
        float3 result = 0;
        bool isHit = false;
        int k = 0;
        while (dot(normal, curRDir) < 0 && k < 3)
        {
        //    float3 noiseValue3 = tex2D(texNoise, input.TexCoord * 5.0 * (k + 1 + 1) + GameTime * 8 * (k + 1)).rgb;
            noiseValue2 = tex2D(texNoise, input.TexCoord * 5.0 * (0 + 1 + 1) + GameTime * 8 * (k + 1)).rg;
            float3 importanceSampleDir = ImportanceSampleGGX(noiseValue2.xy, rDir, mer.z);
    
    
            curRDir = importanceSampleDir;
            marchPos = worldPos + curRDir * 0.01;
            k++;
      

        }
         [unroll]
        for (int i = 0; i < 20; i++)
        {
            if (dot(normal, curRDir) < 0)
            {
            
            isHit = false;
            break;
        }
            marchPos += (curRDir) * 0.3 * /*(pow((i + 1), 1.41)*/noiseValue1 * rayLengthAmp * strideLen;
     //   ssrThickness += (0.1);
   /*     float4 offset = float4(marchPos, 1.0);
        offset = mul(offset, ViewProjection);
        offset.xyz /= offset.w;
        offset.xy = offset.xy * 0.5 + 0.5 + (offset.z * 0.0000000001);
        offset.y = 1 - offset.y;*/
            float2 uv = GetScreenCoordFromWorldPos(marchPos);
    /*    float4 marchDepthView = mul(float4(marchPos, 1), View);
       
        marchDepthView.z = marchDepthView.z + marchDepthView.x * 0.00001 + marchDepthView.y * 0.000001;*/
            float testDepth = GetViewDepthFromWorldPos(marchPos);
        
       // float3 worldPosSampled = PositionWSTex.Sample(defaultSampler, uv.xy).xyz;
     /*   float4 sampleViewPosDepth = mul(float4(worldPosSampled, 1), View);
        sampleViewPosDepth.z = sampleViewPosDepth.z + sampleViewPosDepth.x * 0.000001 + sampleViewPosDepth.y * 0.0000001;*/
        
            float sampleDepthM0 = tex2D(gProjectionDepthM0, uv.xy).x;

            float sampleDepthM1 = tex2D(gProjectionDepthM1, uv.xy).x;

            float sampleDepthM2 = tex2D(gProjectionDepthM2, uv.xy).x;

            float sampleDepthM3 = tex2D(gProjectionDepthM3, uv.xy).x;

            float sampleDepthM4 = tex2D(gProjectionDepthM4, uv.xy).x;

            float sampleDepthM5 = tex2D(gProjectionDepthM5, uv.xy).x;
        
        
        
            float sampleDepthArray[6] = { sampleDepthM0, sampleDepthM1, sampleDepthM2, sampleDepthM3, sampleDepthM4, sampleDepthM5 }; /*GetViewDepthFromWorldPos(worldPosSampled)*/ //tex2D(gProjectionDepth,uv.xy).x;
            float sampleDepth = sampleDepthArray[miplevel];
            if ((uv.x) < 0 || (uv.y) < 0 || (uv.x) > 1 || (uv.y) > 1)
            {
                isHit = false;
                break;
            }
     //   if (sampleDepth <0.01||testDepth<0.01)
     //   {
     //       return float4(0, 0, 0, 1);
      //  }
            if (testDepth > sampleDepth)
            {
                if (testDepth > sampleDepth && abs((testDepth) - (sampleDepth)) < 0.4 * noiseValue1 * rayLengthAmp /*length(preMarchPos-marchPos)*/ && miplevel <= 0)
                {
             //   return float4(testDepth/1000.0,0, 0, 1);
                    float3 finalPoint = preMarchPos;
                    float _sign = 1.0;
                    float3 direction = (marchPos - preMarchPos);
                    float2 uv1 = 0;
                    float3 worldPosSampled1 = 0;

                    float testDepth1 = 0;

                    float sampleDepth1 = 0;
         //   for (int j = 0; j < 2; j++)
          //  {
            
             
                    direction *= 0.5;
                    finalPoint += direction * _sign;
                    uv1 = GetScreenCoordFromWorldPos(finalPoint);
               // worldPosSampled1 = PositionWSTex.Sample(defaultSampler, uv1.xy).xyz;
                    testDepth1 = GetViewDepthFromWorldPos(finalPoint);
                    sampleDepth1 = tex2D(gProjectionDepthM0, uv1.xy).x; // GetViewDepthFromWorldPos(worldPosSampled1);
                    _sign = -sign(testDepth1 - sampleDepth1);
                    
                    direction *= 0.5;
                    finalPoint += direction * _sign;
                    uv1 = GetScreenCoordFromWorldPos(finalPoint);
            //    worldPosSampled1 = PositionWSTex.Sample(defaultSampler, uv1.xy).xyz;
                    testDepth1 = GetViewDepthFromWorldPos(finalPoint);
                    sampleDepth1 = tex2D(gProjectionDepthM0, uv1.xy).x; // GetViewDepthFromWorldPos(worldPosSampled1);
                    _sign = -sign(testDepth1 - sampleDepth1);
            
                    direction *= 0.5;
                    finalPoint += direction * _sign;
                    uv1 = GetScreenCoordFromWorldPos(finalPoint);
            //    worldPosSampled1 = PositionWSTex.Sample(defaultSampler, uv1.xy).xyz;
                    testDepth1 = GetViewDepthFromWorldPos(finalPoint);
                    sampleDepth1 = tex2D(gProjectionDepthM0, uv1.xy).x; // GetViewDepthFromWorldPos(worldPosSampled1);
                    _sign = -sign(testDepth1 - sampleDepth1);
                
                    direction *= 0.5;
                    finalPoint += direction * _sign;
                    uv1 = GetScreenCoordFromWorldPos(finalPoint);
            //    worldPosSampled1 = PositionWSTex.Sample(defaultSampler, uv1.xy).xyz;
                    testDepth1 = GetViewDepthFromWorldPos(finalPoint);
                    sampleDepth1 = tex2D(gProjectionDepthM0, uv1.xy).x; // GetViewDepthFromWorldPos(worldPosSampled1);
                    _sign = -sign(testDepth1 - sampleDepth1);
            
              
                 
               
            
             
           
      //      }
           //     uv1 = GetScreenCoordFromWorldPos(marchPos);
            
                    float3 lum = LumTex.Sample(defaultSampler, uv1.xy).xyz;
           
                    float3 specular = lum * (F * brdf.x + brdf.y);
                    isHit = true;
                    result = specular;
                    break;
                //    return float4(lerp(specular, prevColor, clamp(blendFactor, 0.1, 0.9)), 1);

                }
                miplevel--;
                marchPos -= (curRDir) * 0.3 * /*(pow((i + 1), 1.41)*/noiseValue1 * rayLengthAmp * strideLen;
                strideLen /= 2.0;
         
            
            }
            else
            {
                if (miplevel < 5)
                {
                    miplevel++;
                    strideLen *= 2;
                    isRayReturning = false;

                }
            
            }
            preMarchPos = marchPos;
       

        }
    
        float3 R = reflect(-V, normal);
        R = normalize(R);
        if (isHit == true)
        {
            finalColor += result;

        }
        else
        {
            const float MAX_REFLECTION_LOD = 4.0;
            float3 prefilteredColor = lerp(texCUBElod(preFilteredSpecularSampler, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, texCUBElod(preFilteredSpecularSamplerNight, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, mixValue);
         //   float2 brdf = tex2D(texBRDFLUT, float2(max(dot(normal, V), 0.0), 1 - mer.z)).rg;
            float3 specularEnv = prefilteredColor * (F * brdf.x + brdf.y);
    
            finalColor += specularEnv;

        }
    
   
    
  //  finalColor /= 1;
                finalColor = lerp(finalColor, prevColor,0.6);
   
                return float4(finalColor.xyz, 1);
   
}










float4 MainPSNew(VertexShaderOutput input) : COLOR
{
    
    float linearDepth = 0;
    linearDepth = tex2D(gProjectionDepthM0, input.TexCoord);
    if (linearDepth >= 900 || linearDepth <= 0.1)
    {
        discard;
    }
    
    
    float3 worldPos = ReconstructViewPos(input.TexCoord, linearDepth) + CameraPos; // PositionWSTex.Sample(defaultSampler, input.TexCoord).xyz;
    
  //  return float4(worldPos.xyz, 1);
    float3 normal = NormalTex.Sample(defaultSampler, input.TexCoord).xyz * 2 - 1;
    float3 mer = tex2D(MERSampler, input.TexCoord).xyz;
    
    
    
    

  //  float noiseValue = worldPos.x;
    worldPos = worldPos + normal * 0.5 * length(worldPos - CameraPos) / 100;
    
    
    float3 vDir = normalize(worldPos - CameraPos);
    float3 rDir = reflect(vDir, (normal));
    
    float3 rayOrigin = worldPos;
    float NdotL =max(dot((normal), normalize(-vDir)), 0.0);
    NdotL =1.0/NdotL;
     
    float rayLengthAmp = max(NdotL / 3.0, 1.0);
   // return float4(rayLengthAmp.xxx, 1);
    float ssrThickness = 0.2;
   
    bool isRayReturning = false;

   
  //  return float4(noiseValue1.xxx, 1);
   
    
    
    float2 prevTexCoord = input.TexCoord + tex2D(motionVectorTex, input.TexCoord).xy;
    float maxBlendDistance = length(PixelSize) * 2;
    float deltaLength = length(prevTexCoord - input.TexCoord);
    float blendFactor = clamp(deltaLength / maxBlendDistance, 0, 1);
    float3 prevColor = prevTexCoord.x > 0 && prevTexCoord.y > 0 && prevTexCoord.x < 1 && prevTexCoord.y < 1 ? tex2D(prevSSRTex, prevTexCoord).xyz : 0;
    
    
    
 
    //float2 brdf1 = tex2D(texBRDFLUT, input.TexCoord.xy).rg;
   // return float4(brdf1, 0, 1);
    
  
   
             

            
    float3 N = normal;
    float3 V = normalize(CameraPos - worldPos);
    float3 F0 = float3(0.04, 0.04, 0.04);
    F0 = lerp(F0, pow(tex2D(gAlbedo, input.TexCoord).xyz, 2.2), mer.x);
    float3 F = fresnelSchlickRoughness(max(dot(N, V), 0.0), F0, mer.z);
       //   Lo = Lo / (Lo + float3(1.0, 1.0, 1.0));
       //    Lo = pow(Lo, float3(1.0 / 1, 1.0 / 1, 1.0 / 1));
            
    
    
    
    float2 brdf = tex2D(texBRDFLUT, float2(max(dot(N, V), 0.0), 1 - mer.z)).rg;
    
    float3 finalColor = 0;
   if (mer.z > 0.8f)
    {
        float3 R = reflect(-V, normal);
        R = normalize(R);
        const float MAX_REFLECTION_LOD = 4.0;
        float3 prefilteredColor = lerp(texCUBElod(preFilteredSpecularSampler, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, texCUBElod(preFilteredSpecularSamplerNight, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, mixValue);
       
        float3 specularEnv = prefilteredColor * (F * brdf.x + brdf.y);
    
      //  finalColor += specularEnv;
     
   
        return float4(specularEnv.xyz, 1);
        
        
    }
  
    
 
    
     
        
    
    float3 curRDir = rDir;
    float noiseValue1 = tex2D(texNoise, input.TexCoord * 4.0 + GameTime * 5).g +0.9;
    
    
    
    
    
     
        
     
     
     //   float3 noiseValue4 = tex2D(texNoise, input.TexCoord * 6.0*(0+1) + GameTime * 9).rgb * 2 - 1;
    
  /*     float3 randomVec = float3(noiseValue2.rg, 0);
        float3 tangent = normalize(randomVec - curRDir * dot(randomVec, curRDir));
        float3 bitangent = cross(curRDir, tangent);
        float3x3 TBN = float3x3(tangent, bitangent, curRDir);
        float3 noiseValue3 = float3(noiseValue4.rg * mer.z * 0.1, noiseValue2.b * 0.5 + 0.5);
        float3 rayRoughnessAmp = mul(noiseValue3, TBN);
    
     
        curRDir = rayRoughnessAmp;
        curRDir = normalize(curRDir);*/
    float2 noiseValue2 = tex2D(texNoise, input.TexCoord * 5.0 * (0 + 1 + 1) + GameTime * 8 * (0 + 1)).rg;
    float3 importanceSampleDir = ImportanceSampleGGX(noiseValue2.xy, rDir, mer.z);
    
    
    curRDir = importanceSampleDir;
    float3 marchPos = worldPos + curRDir * 0.01;
    float3 preMarchPos = rayOrigin;
 
    int k = 0;
    while (dot(normal, curRDir) < 0 && k < 3)
    {
        //    float3 noiseValue3 = tex2D(texNoise, input.TexCoord * 5.0 * (k + 1 + 1) + GameTime * 8 * (k + 1)).rgb;
        noiseValue2 = tex2D(texNoise, input.TexCoord * 5.0 * (0 + 1 + 1) + GameTime * 8 * (k + 1)).rg;
        float3 importanceSampleDir = ImportanceSampleGGX(noiseValue2.xy, rDir, mer.z);
    
    
        curRDir = importanceSampleDir;
        marchPos = worldPos + curRDir * 0.01;
        k++;
      

    }
    float maxDist = 100.0f;
    float3 viewRDir = normalize(mul(float4(rDir, 1), ViewOrigin).xyz);
    
    float3 viewPosOrigin = mul(float4(worldPos, 1), View);
   
   
    float end = viewPosOrigin.z + viewRDir.z * maxDist;
    if (end >-0.1)
    {
        maxDist = abs(-0.1 - viewPosOrigin.z) / viewRDir.z;
    }
     
    float3 viewPosEnd = viewPosOrigin + viewRDir * maxDist;
 //   float4 projPos = mul(float4(viewPosEnd, 1), Projection);
   // return float4((projPos.xy / projPos.w) * 0.5 + 0.5, 0, 1);
    float4 startHScreen = TransformViewToHScreen(viewPosOrigin, 1.0/(PixelSize.xy));
    float4 endHScreen = TransformViewToHScreen(viewPosEnd, 1.0 / (PixelSize.xy));
  //  return float4((endHScreen.xy / endHScreen.w) * PixelSize, 0, 1);
    float startK = 1.0 / startHScreen.w;
    float endK = 1.0 /endHScreen.w;
    float2 startScreen = startHScreen.xy * startK;
    float2 endScreen = endHScreen.xy * endK;
    
    float3 startQ = viewPosOrigin * startK;

    float3 endQ =viewPosEnd * endK;
    
    
    float2 diff = endScreen - startScreen;
    bool permute = false;
    if (abs(diff.x) < abs(diff.y))
    {
        permute = true;

        diff = diff.yx;
        startScreen = startScreen.yx;
        endScreen = endScreen.yx;
    }
    float dir = sign(diff.x);
    float invdx = dir / diff.x;
    float2 dp = float2(dir, invdx * diff.y);
    float3 dq = (endQ - startQ) * invdx;
    float dk = (endK - startK) * invdx;
    
   // return float4(dq.z*10000, 0, 0, 1);
    dp *= 1;
    dq *= 1;
    dk *= 1;
    
    float rayZMin = viewPosOrigin.z;
    float rayZMax = viewPosOrigin.z;
    float preZ = viewPosOrigin.z;

    float2 P = startScreen;
    P = floor(P);
    float3 Q = startQ;
    float K = startK;

    end = endScreen.x * dir;
   /* if ((endScreen.xy * PixelSize).x < 0 || (endScreen.xy * PixelSize).x > 1 || (endScreen.xy * PixelSize).y < 0 || (endScreen.xy *PixelSize).y > 1)
    {
        return float4(0, 0, 0, 1);
    }
    return float4(endScreen.xy*PixelSize, 0, 1);*/
    int miplevel = 0;
    float strideLen =1 ;
    float3 result = 0;
    bool isHit = false;
    float traceStepCount = 0;
    int unHitErrCode = 0;
    float2 textureSize = 1.0 / PixelSize;
    [loop]
    for (int i = 0; i <35; i++)
    {
        traceStepCount++;
        P += dp * strideLen ;
        Q.z += dq.z * strideLen ;
        K += dk * strideLen ;
        rayZMin = preZ;
        rayZMax = (dq.z * 0.5 + Q.z) / (dk * 0.5 + K);
        preZ = rayZMax;
        if (rayZMin > rayZMax)
        {
            swap(rayZMin, rayZMax);
        }
        
        float2 hitUV = permute ? P.yx : P;
        hitUV *= PixelSize;
        
        if ((hitUV.x < 0.0) || (hitUV.y < 0.0) || (hitUV.x > 1.0) || (hitUV.y > 1.0))
        {
           
            isHit = false;
            unHitErrCode = 1;
            break;
        }
     
        float sampleDepthM0 = -tex2D(gProjectionDepthM0, hitUV.xy).x;

        float sampleDepthM1 = -tex2D(gProjectionDepthM1, hitUV.xy).x;

        float sampleDepthM2 = -tex2D(gProjectionDepthM2, hitUV.xy).x;

        float sampleDepthM3 = -tex2D(gProjectionDepthM3, hitUV.xy).x;

        float sampleDepthM4 = -tex2D(gProjectionDepthM4, hitUV.xy).x;

        float sampleDepthM5 = -tex2D(gProjectionDepthM5, hitUV.xy).x;
   
      
        
        
        float sampleDepthArray[6] = { sampleDepthM0, sampleDepthM1, sampleDepthM2, sampleDepthM3, sampleDepthM4, sampleDepthM5 };
      
        
        float surfaceDepth = sampleDepthArray[miplevel];
        if (abs(surfaceDepth) / 1000 > 0.99)
        {
           
            isHit = false;
            unHitErrCode = 2;
            break;
        }
        bool isBehind = (rayZMin + 0.01 <= surfaceDepth);
        if (isBehind)
        {
            bool intersecting = isBehind; //&& (abs(rayZMax - surfaceDepth) < 0.26 * rayLengthAmp);

            if (intersecting&&miplevel<=0)
            {
                float3 lum = LumTex.Sample(defaultSampler, hitUV.xy).xyz;
           
                float3 specular = lum * (F * brdf.x + brdf.y);
                isHit = true;
                result = specular;
                break;
            }
            P -= dp * strideLen ;
            Q.z -= dq.z * strideLen ;
            K -= dk * strideLen ;
          
            miplevel = clamp(miplevel-1, 0, 5);
            strideLen /= 2.0;
        }
        else
        {
            if (miplevel <5)
            {
                miplevel++;
                        
                strideLen *= 2.0;
            }
        }
        
        
        
    /*    if (dot(normal, curRDir) < 0)
        {
            
            isHit = false;
            break;
        }*/
     //   marchPos += (curRDir) * 0.3 * /*(pow((i + 1), 1.41)*/noiseValue1 * rayLengthAmp * strideLen;
     //   ssrThickness += (0.1);
   /*     float4 offset = float4(marchPos, 1.0);
        offset = mul(offset, ViewProjection);
        offset.xyz /= offset.w;
        offset.xy = offset.xy * 0.5 + 0.5 + (offset.z * 0.0000000001);
        offset.y = 1 - offset.y;*/
     /*   P += dp *1;
        Q.z += dq.z * 1;
        K += dk *1;
    
         
       // rayZMin = (Q.z) / ( K);
        rayZMin = preZ;
        rayZMax = (dq.z * 0.5 + Q.z) / (dk * 0.5 + K);
        preZ = rayZMax;
        if (rayZMin > rayZMax)
        {
            swap(rayZMin, rayZMax);
        }
          
        
        float2 hitUV = permute ? P.yx : P;
        hitUV *=PixelSize;
        
        if ((hitUV.x < 0.0) || (hitUV.y < 0.0) || (hitUV.x > 1.0) || (hitUV.y >1.0))
        {
            isHit = false;
            
            break;
        }
    
        float sampleDepthM0 = -tex2D(gProjectionDepthM0, hitUV.xy).x;

    
      
        
        float surfaceDepth = sampleDepthM0;
        if (-surfaceDepth / 1000 > 0.9)
        {
            isHit = false;
            
            break;
        }
        bool isBehind = (rayZMin + 0.05 <= surfaceDepth);
        
     //   float avgTestDepth = (rayZMin + rayZMax) / 2.0f;
        bool intersecting = isBehind && (abs(surfaceDepth - rayZMax) < 0.25*rayLengthAmp);

            if (intersecting)
            {
                float3 lum = LumTex.Sample(defaultSampler, hitUV.xy).xyz;
           
                float3 specular = lum * (F * brdf.x + brdf.y);
                isHit = true;
                result = specular;
                break;
            }
      
       */
         
      
        
   
       

    }
    
    float3 R = reflect(-V, normal);
    R = normalize(R);
    if (isHit == true)
    {
        finalColor +=result;

    }
    else
    {
   /*     if (unHitErrCode == 1)
        {
            return float4(0, 1, 0, 1);
        }
        else if (unHitErrCode == 2)
        {
            return float4(0,0, 1, 1);
        }*/
        const float MAX_REFLECTION_LOD = 4.0;
        float3 prefilteredColor = lerp(texCUBElod(preFilteredSpecularSampler, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, texCUBElod(preFilteredSpecularSamplerNight, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, mixValue);
         //   float2 brdf = tex2D(texBRDFLUT, float2(max(dot(normal, V), 0.0), 1 - mer.z)).rg;
        float3 specularEnv = prefilteredColor * (F * brdf.x + brdf.y);
    
        finalColor += specularEnv;

    }
    
   
    
  //  finalColor /= 1;
   finalColor = lerp(finalColor, prevColor, 0.75);
   
    return float4(finalColor.xyz, 1);
   
    
   
}







float linearDepthToProjectionDepth(float linearDepth, float near, float far)
{
    return (1.0 / linearDepth - 1.0 / near) / (1.0 / far - 1.0 / near);
}
float ProjectionDepthToLinearDepth(float depth, float near, float far)
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));
}
float3 IntersectDepthPlane(float3 RayOrigin, float3 RayDir, float t)
{
    return RayOrigin + RayDir * t;
}

float2 GetCellCount(float2 Size, float Level)
{
    return floor(Size / (Level > 0.0 ? exp2(Level) : 1.0));
}

float2 GetCell(float2 pos, float2 CellCount)
{
    return floor(pos * CellCount);
}
float GetMinimumDepthPlane(float2 p, int mipLevel)
{
    
    float sampleDepthM0 = linearDepthToProjectionDepth(tex2D(gProjectionDepthM0, p.xy).x, 0.1f, 1000.0f);

    float sampleDepthM1 = linearDepthToProjectionDepth(tex2D(gProjectionDepthM1, p.xy).x, 0.1f, 1000.0f);

    float sampleDepthM2 = linearDepthToProjectionDepth(tex2D(gProjectionDepthM2, p.xy).x, 0.1f, 1000.0f);

    float sampleDepthM3 = linearDepthToProjectionDepth(tex2D(gProjectionDepthM3, p.xy).x, 0.1f, 1000.0f);

    float sampleDepthM4 = linearDepthToProjectionDepth(tex2D(gProjectionDepthM4, p.xy).x, 0.1f, 1000.0f);

    float sampleDepthM5 = linearDepthToProjectionDepth(tex2D(gProjectionDepthM5, p.xy).x, 0.1f, 1000.0f);
   
      
        
        
    float sampleDepthArray[6] = { sampleDepthM0, sampleDepthM1, sampleDepthM2, sampleDepthM3, sampleDepthM4, sampleDepthM5 };
    
    return sampleDepthArray[clamp(mipLevel, 0, 5)];

}
float3 IntersectCellBoundary(float3 o, float3 d, float2 cell, float2 cell_count, float2 crossStep, float2 crossOffset)
{
    float3 intersection = 0;
	
    float2 index = cell + crossStep;
    float2 boundary = index / cell_count;
    boundary += crossOffset;
	
    float2 delta = boundary - o.xy;
    delta /= d.xy;
    float t = min(delta.x, delta.y);
	
    intersection = IntersectDepthPlane(o, d, t);
	
    return intersection;
}

bool CrossedCellBoundary(float2 CellIdxA, float2 CellIdxB)
{
    return CellIdxA.x != CellIdxB.x || CellIdxA.y != CellIdxB.y;
}
float4 MainPSRealHiZ(VertexShaderOutput input) : COLOR
{
    
    float linearDepth = 0;
    linearDepth = tex2D(gProjectionDepthM0, input.TexCoord);
    if (linearDepth >= 900 || linearDepth <= 0.1)
    {
        discard;
    }
    
    
    float3 worldPos = ReconstructViewPos(input.TexCoord, linearDepth) + CameraPos; // PositionWSTex.Sample(defaultSampler, input.TexCoord).xyz;
    
  //  return float4(worldPos.xyz, 1);
    float3 normal = NormalTex.Sample(defaultSampler, input.TexCoord).xyz * 2 - 1;
    float3 mer = tex2D(MERSampler, input.TexCoord).xyz;
    
    
    
    

  //  float noiseValue = worldPos.x;
    worldPos = worldPos + normal * 0.3 * length(worldPos - CameraPos) / 100;
    
    
    float3 vDir = normalize(worldPos - CameraPos);
    float3 rDir = reflect(vDir, (normal));
    
    float3 rayOrigin = worldPos;
    float NdotL = 2 - max(dot(normal, -vDir), 0.0) * 1.5;
    NdotL = pow(NdotL, 2);
     
    float rayLengthAmp = 1 + NdotL;
    float ssrThickness = 0.2;
   
    bool isRayReturning = false;


    
    
    float2 prevTexCoord = input.TexCoord + tex2D(motionVectorTex, input.TexCoord).xy;
    float maxBlendDistance = length(PixelSize) * 2;
    float deltaLength = length(prevTexCoord - input.TexCoord);
    float blendFactor = clamp(deltaLength / maxBlendDistance, 0, 1);
    float3 prevColor = prevTexCoord.x > 0 && prevTexCoord.y > 0 && prevTexCoord.x < 1 && prevTexCoord.y < 1 ? tex2D(prevSSRTex, prevTexCoord).xyz : 0;
    
    
    
 
  
   
             

            
    float3 N = normal;
    float3 V = normalize(CameraPos - worldPos);
    float3 F0 = float3(0.04, 0.04, 0.04);
    F0 = lerp(F0, pow(tex2D(gAlbedo, input.TexCoord).xyz, 2.2), mer.x);
    float3 F = fresnelSchlickRoughness(max(dot(N, V), 0.0), F0, mer.z);
  
    
    
    float2 brdf = tex2D(texBRDFLUT, float2(max(dot(N, V), 0.0), 1 - mer.z)).rg;
    
    float3 finalColor = 0;
 
    
 
    
     
        
    
    float3 curRDir = rDir;
    float noiseValue1 = tex2D(texNoise, input.TexCoord * 4.0 + GameTime * 5).g + 0.5;
    
    
    
    
    
     
        
  
    float2 noiseValue2 = tex2D(texNoise, input.TexCoord * 5.0 * (0 + 1 + 1) + GameTime * 8 * (0 + 1)).rg;
    float3 importanceSampleDir = ImportanceSampleGGX(noiseValue2.xy, rDir, mer.z);
    
    
    curRDir = importanceSampleDir;
  
   
    int miplevel = 0;
   
    float3 result = 0;
   
    int k = 0;
    while (dot(normal, curRDir) < 0 && k < 3)
    {
        //    float3 noiseValue3 = tex2D(texNoise, input.TexCoord * 5.0 * (k + 1 + 1) + GameTime * 8 * (k + 1)).rgb;
        noiseValue2 = tex2D(texNoise, input.TexCoord * 5.0 * (0 + 1 + 1) + GameTime * 8 * (k + 1)).rg;
        float3 importanceSampleDir = ImportanceSampleGGX(noiseValue2.xy, rDir, mer.z);
    
    
        curRDir = importanceSampleDir;
     
        k++;
      

    }
    float2 textureSize = 1.0 / PixelSize;
    float maxDist = 100.0f;
    float3 viewRDir = normalize(mul(float4(curRDir, 1), ViewOrigin).xyz);
    
    float3 viewPosOrigin = mul(float4(worldPos, 1), View);
   
   
    float end = viewPosOrigin.z + viewRDir.z * maxDist;
    if (end > -0.1)
    {
        maxDist = abs(-0.1 - viewPosOrigin.z) / viewRDir.z;
    }
     
    float3 viewPosEnd = viewPosOrigin + viewRDir * maxDist;
    float4 startHScreen = TransformViewToHScreen(viewPosOrigin, 1.0 / (PixelSize.xy));
    float4 endHScreen = TransformViewToHScreen(viewPosEnd, 1.0 / (PixelSize.xy));
  //  return float4((endHScreen.xy / endHScreen.w) * PixelSize, 0, 1);
    float startK = 1.0 / startHScreen.w;
    float endK = 1.0 / endHScreen.w;
    float3 startScreen = startHScreen.xyz * startK;
    
    float3 startScreenTextureSpace = float3(startScreen.xy * PixelSize, startScreen.z);
    
    float3 endScreen = endHScreen.xyz * endK;
    float3 endScreenTextureSpace = float3(endScreen.xy * PixelSize, endScreen.z);
    float3 reflectDirTextureSpace = normalize(endScreenTextureSpace - startScreenTextureSpace);
    
    
    
    float outMaxDistance = reflectDirTextureSpace.x >= 0 ? (1 - startScreenTextureSpace.x) / reflectDirTextureSpace.x : -startScreenTextureSpace.x / reflectDirTextureSpace.x;
    outMaxDistance = min(outMaxDistance, reflectDirTextureSpace.y < 0 ? (-startScreenTextureSpace.y / reflectDirTextureSpace.y) : ((1 - startScreenTextureSpace.y) / reflectDirTextureSpace.y));
    outMaxDistance = min(outMaxDistance, reflectDirTextureSpace.z < 0 ? (-startScreenTextureSpace.z / reflectDirTextureSpace.z) : ((1 - startScreenTextureSpace.z) / reflectDirTextureSpace.z));
 
    
 /*   float4 rayPosInTS = float4(startScreenTextureSpace.xyz + dp, 0);
    float4 vRayDirInTS = float4(dp.xyz, 0);
    float4 rayStartPos = rayPosInTS;*/
  
    int maxLevel = 5;
    float2 crossStep = float2(reflectDirTextureSpace.x >= 0 ? 1 : -1, reflectDirTextureSpace.y >= 0 ? 1 : -1);
    float2 crossOffset = crossStep / (1.0 / (PixelSize.xy)) / 128;
    crossStep = saturate(crossStep);
        
    float3 ray = startScreenTextureSpace.xyz;
    float minZ = ray.z;
    float maxZ = ray.z + reflectDirTextureSpace.z * outMaxDistance;
    
    float deltaZ = (maxZ - minZ);

    float3 o = ray;
    float3 d = reflectDirTextureSpace * outMaxDistance;
    
    
    int startLevel = 1;
    int stopLevel = 0;
    
    
    float2 startCellCount = GetCellCount(1.0 / (PixelSize.xy), startLevel);
	
    float2 rayCell = GetCell(ray.xy, startCellCount);
    ray = IntersectCellBoundary(o, d, rayCell, startCellCount, crossStep, crossOffset);
    
    int level = startLevel;
    uint iter = 0;
    bool isBackwardRay = reflectDirTextureSpace.z < 0;
    float rayDir = isBackwardRay ? -1 : 1;
    bool isIntersecting = false;
    [loop]
    while (level >= stopLevel && ray.z * rayDir <= maxZ * rayDir && iter < 50)
    {
        
        float2 cellCount = GetCellCount(1.0 / (PixelSize.xy),level);
         float2 oldCellIdx = GetCell(ray.xy, cellCount);
        
        float cell_minZ = GetMinimumDepthPlane((oldCellIdx + 0.5f) / cellCount, level);
        
        float3 tmpRay = ((cell_minZ > ray.z) && !isBackwardRay) ? IntersectDepthPlane(o, d,(cell_minZ - minZ) / deltaZ) : ray;
        
         float2 newCellIdx = GetCell(tmpRay.xy, cellCount);
        
        float thickness = 0;
        
        if (level == 0)
        {
            thickness = abs(ProjectionDepthToLinearDepth(ray.z, 0.1f, 1000.0f)
             - ProjectionDepthToLinearDepth(cell_minZ, 0.1f, 1000.0f));

        }
        else
        {
            thickness = 0;
        }
        
        bool crossed = (isBackwardRay && (cell_minZ > ray.z))  || CrossedCellBoundary(oldCellIdx, newCellIdx);
      
        if (crossed == true)
        {
            ray = IntersectCellBoundary(o, d, oldCellIdx, cellCount, crossStep, crossOffset);
            level = min((float) maxLevel, level + 1.0f);
         

        }
        else
        {
            ray = tmpRay;
            level = level - 1;
          

        }
      
        if (ray.x < 0 || ray.y < 0 || ray.x > 1 || ray.y > 1)
        {
            isIntersecting = false;
         
        }
         
        
        if (level <= 0 )
        {
            float rayZLinear = ProjectionDepthToLinearDepth(ray.z, 0.1f, 1000.0f);
            float cellMinZLinear = ProjectionDepthToLinearDepth(cell_minZ, 0.1f, 1000.0f);
            
            if (thickness < 0.2 && rayZLinear > cellMinZLinear - 0.02&&rayZLinear<900.0f&&cellMinZLinear<900.0f)
            {
                isIntersecting = true;
            }
            else
            {
                isIntersecting = false;
            }
          
        }
        ++iter;
    }
    float2 uv = ray.xy;
    
    float3 R = reflect(-V, normal);
    R = normalize(R);
    if (isIntersecting == true)
    {
        float3 lum = LumTex.Sample(defaultSampler, uv.xy).xyz;
           
        float3 specular = lum * (F * brdf.x + brdf.y);
         
        result = specular;
        finalColor += result;

    }
    else
    {
        const float MAX_REFLECTION_LOD = 4.0;
        float3 prefilteredColor = lerp(texCUBElod(preFilteredSpecularSampler, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, texCUBElod(preFilteredSpecularSamplerNight, float4(R, mer.z * MAX_REFLECTION_LOD)).rgb, mixValue);
         //   float2 brdf = tex2D(texBRDFLUT, float2(max(dot(normal, V), 0.0), 1 - mer.z)).rg;
        float3 specularEnv = prefilteredColor * (F * brdf.x + brdf.y);
    
        finalColor += specularEnv;

    }
    
   
    
  //  finalColor /= 1;
    finalColor = lerp(finalColor, prevColor, 0.6);
   
        return float4(finalColor.xyz, 1);
   
   
}


technique SSR
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPSRealHiZ();
    }
};