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
float4x4 matInverseProjection;
float4x4 matView;
float4x4 matProjection;
 
matrix ViewProjection;
matrix View;
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
  
    
        
        
        float3 curRDir = rDir;
        float noiseValue1 = tex2D(texNoise, input.TexCoord * 4.0 + GameTime * 5).g + 0.5;
        float3 noiseValue2 = tex2D(texNoise, input.TexCoord * 3.0 * (0 + 1) + GameTime * 8).rgb * 2 - 1;
        float3 noiseValue4 = tex2D(texNoise, input.TexCoord * 6.0*(0+1) + GameTime * 9).rgb * 2 - 1;
    
        float3 randomVec = float3(noiseValue2.rg, 0);
        float3 tangent = normalize(randomVec - curRDir * dot(randomVec, curRDir));
        float3 bitangent = cross(curRDir, tangent);
        float3x3 TBN = float3x3(tangent, bitangent, curRDir);
        float3 noiseValue3 = float3(noiseValue4.rg * mer.z * 0.05, noiseValue2.b * 0.5 + 0.5);
        float3 rayRoughnessAmp = mul(noiseValue3, TBN);
        curRDir = rayRoughnessAmp;
        curRDir = normalize(curRDir);
        float3 marchPos = worldPos + curRDir * 0.01;
        float3 preMarchPos = rayOrigin;
        int miplevel = 0;
        float strideLen = 1;
        float3 result = 0;
        bool isHit = false;
         [unroll]
        for (int i = 0; i < 24; i++)
        {
            if (dot(normal, curRDir) < 0)
            {
                return float4(0, 0, 0, 1);
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
        
        
        
            float sampleDepthArray[6] = { sampleDepthM0, sampleDepthM1, sampleDepthM2, sampleDepthM3, sampleDepthM4, sampleDepthM5}; /*GetViewDepthFromWorldPos(worldPosSampled)*/ //tex2D(gProjectionDepth,uv.xy).x;
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
                if (testDepth > sampleDepth && abs((testDepth) - (sampleDepth)) < 0.5 * noiseValue1 * rayLengthAmp /*length(preMarchPos-marchPos)*/ && miplevel <= 0)
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
                float2 brdf = tex2D(texBRDFLUT, float2(max(dot(normal, V), 0.0), 1 - mer.z)).rg;
                float3 specularEnv = prefilteredColor * (F * brdf.x + brdf.y);
    
                finalColor = specularEnv;

                }
   
                finalColor = lerp(finalColor, prevColor, 0.5);
   
                return float4(finalColor.xyz, 1);
   
}
technique SSR
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};