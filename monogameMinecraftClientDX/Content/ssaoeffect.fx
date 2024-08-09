  
#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif
#if MGFX
// This unused parameter helps avoiding crashes due to compiler optimizations in monogame
float4 Float4Parameter0 : Float4Parameter0;
#endif
 
sampler2D gNormal = sampler_state
{
    Texture = <NormalTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D gProjectionDepth = sampler_state
{
    Texture = <ProjectionDepthTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
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
sampler2D gAlbedo = sampler_state
{
    Texture = <AlbedoTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
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
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = input.Position;
    output.TexCoords = float2(input.TexCoords.x, input.TexCoords.y);

    return output;
}


sampler2D texNoise = sampler_state
{
    Texture = <NoiseTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};


 float4 ProjectionParams2;
 float4 CameraViewTopLeftCorner;
 float4 CameraViewXExtent;
 float4 CameraViewYExtent;

float3 CameraPos;
matrix View;
matrix ViewProjection;

float3 samples[8];

float3 ReconstructViewPos(float2 uv, float linearEyeDepth)
{
  //  uv.y = 1.0 - uv.y;
    float zScale = linearEyeDepth * ProjectionParams2.x; // divide by near plane  
    float3 viewPos = CameraViewTopLeftCorner.xyz + CameraViewXExtent.xyz * uv.x + CameraViewYExtent.xyz * uv.y;
    viewPos *= zScale;
    return viewPos;
}



float LinearizeDepth(float depth)
{
    float NEAR = 0.1;
    float FAR = 500.0f;
    float d = depth;
    return 1.0 / (d * (1/FAR-1/NEAR)+1/NEAR);
}
float LinearizeDepth1(float depth)
{
    
    return depth*50;
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
float2 PixelSize;
#define PI 3.1415926
#define STEP_COUNT 8
#define DIRECTION_COUNT 8
#define RADIUS 0.5
float FallOff(float dist)
{
    return 1 - dist * dist / (RADIUS * RADIUS);
}
 float ComputeAO(float3 vpos, float3 stepVpos, float3 normal, inout float topOcclusion)
{
    
    
    float3 h = stepVpos - vpos;
    float dist = length(h);
    
    float occlusion = dot((normal), h)/dist;
     
    float diff = max(occlusion - topOcclusion, 0);
    topOcclusion = max(occlusion, topOcclusion);
 //   return max( occlusion * saturate(FallOff(dist)), topOcclusion);
    return diff * saturate(FallOff(dist));
}
float4x4 NormalView;



float2 SearchForLargestAngleDual(uint NumSteps, float2 BaseUV, float2 ScreenDir, float SearchRadius, float InitialOffset, float3 ViewPos, float3 ViewDir, float AttenFactor)
{
    float SceneDepth, LenSq, OOLen, Ang, FallOff;
    float3 V;
    float2 SceneDepths = 0;

    float2 BestAng = float2(-1, -1);
    float Thickness =0.9;

    [unroll(STEP_COUNT)]
    for (uint i = 0; i < NumSteps; i++)
    {
        float fi = (float) i;

        float2 UVOffset = ScreenDir * max(SearchRadius * (fi + InitialOffset), (fi + 1));
        UVOffset.y *= -1;
        float4 UV2 = BaseUV.xyxy + float4(UVOffset.xy, -UVOffset.xy);

	// Positive Direction
        float depthh1 = tex2D(gProjectionDepth, UV2.xy).x;
        float3 worldPosh1 = ReconstructViewPos(UV2.xy, depthh1 > 0.01 ? depthh1 : 1000) + CameraPos;
        V = mul(float4(worldPosh1,1),View).xyz - ViewPos;
        LenSq = dot(V, V);
        OOLen = rsqrt(LenSq + 0.0001);
        Ang = dot(V, ViewDir) * OOLen;

        FallOff = saturate(LenSq * AttenFactor);
        Ang = lerp(Ang, BestAng.x, FallOff);
        BestAng.x = (Ang > BestAng.x) ? Ang : lerp(Ang, BestAng.x, Thickness);

	// Negative Direction
        float depthh2 = tex2D(gProjectionDepth, UV2.zw).x;
        float3 worldPosh2 = ReconstructViewPos(UV2.zw, depthh2 > 0.01 ? depthh2 : 1000) + CameraPos;
        V = mul(float4(worldPosh2, 1), View).xyz - ViewPos;
        LenSq = dot(V, V);
        OOLen = rsqrt(LenSq + 0.0001);
        Ang = dot(V, ViewDir) * OOLen;

        FallOff = saturate(LenSq * AttenFactor);
        Ang = lerp(Ang, BestAng.y, FallOff);

        BestAng.y = (Ang > BestAng.y) ? Ang : lerp(Ang, BestAng.y, Thickness);
    }

    BestAng.x = acos(clamp(BestAng.x, -1.0, 1.0));
    BestAng.y = acos(clamp(BestAng.y, -1.0, 1.0));

    return BestAng;
}

float ComputeInnerIntegral(float2 Angles, float2 ScreenDir, float3 ViewDir, float3 ViewSpaceNormal)
{
	// Given the angles found in the search plane 
	// we need to project the View Space Normal onto the plane 
	// defined by the search axis and the View Direction and perform the inner integrate
    float3 PlaneNormal = normalize(cross(float3(ScreenDir, 0), ViewDir));
    float3 Perp = cross(ViewDir, PlaneNormal);
    float3 ProjNormal = ViewSpaceNormal - PlaneNormal * dot(ViewSpaceNormal, PlaneNormal);

    float LenProjNormal = length(ProjNormal) + 0.000001f;
    float RecipMag = 1.0f / (LenProjNormal);

    float CosAng = dot(ProjNormal, Perp) * RecipMag;
    float Gamma = acos(CosAng) - PI/2.0;
    float CosGamma = dot(ProjNormal, ViewDir) * RecipMag;
    float SinGamma = CosAng * -2.0f;

	// clamp to normal hemisphere 
    Angles.x = Gamma + max(-Angles.x - Gamma, -(PI / 2.0));
    Angles.y = Gamma + min(Angles.y - Gamma, (PI / 2.0));

    float AO = ((LenProjNormal) * 0.25 *
			((Angles.x * SinGamma + CosGamma - cos((2.0 * Angles.x) - Gamma)) +
			(Angles.y * SinGamma + CosGamma - cos((2.0 * Angles.y) - Gamma))));

    return AO;
}

 
float4 MainPS(VertexShaderOutput input):SV_Target0
{
    float linearDepth =tex2D(gProjectionDepth, input.TexCoords).x;
   
    float3 normal = tex2D(gNormal, input.TexCoords).xyz * 2.0 - 1.0;
    normal = mul(float4(normal, 0), NormalView).xyz;
    float3 worldPos = /*!usingDepthReconstructWorldPos?tex2D(gPositionWS, input.TexCoords).xyz:*/ReconstructViewPos(input.TexCoords, linearDepth) + CameraPos;
   // return float4(worldPos.xyz, 1);
    float3 viewPos = mul(float4(worldPos, 1), View).xyz;
 
    
     
    float stride = ((1 / PixelSize.x)*RADIUS / -viewPos.z) / (STEP_COUNT + 1.0);
    float stepRadian = PI*2.0 / DIRECTION_COUNT;
    if (stride < 1)
    {
        return 1.0;
    }
        
    float3 randomVec = float3(tex2D(texNoise, input.TexCoords).r , tex2D(texNoise, input.TexCoords).g, 0);
    float ao = 0;
    float2 ScreenDir = randomVec.xy * stride*PixelSize;
    float Offset = tex2D(texNoise, input.TexCoords).b;
    float SinDeltaAngle = sin(PI * 2.0 / (float) DIRECTION_COUNT);
    float CosDeltaAngle = cos(PI * 2.0 / (float) DIRECTION_COUNT);
   float PixelRadius =min(RADIUS / -viewPos.z, 0.01);
    float StepRadius = PixelRadius / ((float) STEP_COUNT + 1);
    
    float WorldRadius = 30.0f;
    float AttenFactor = 2.0 / (WorldRadius * WorldRadius);
    [unroll(DIRECTION_COUNT)]
    for (int i = 0; i < DIRECTION_COUNT; i++)
    {
    /*  float radian = stepRadian * ( i + randomVec.x);
        float sinr, cosr;
        sincos(radian, sinr, cosr);
        float2 direction = float2(cosr, sinr) * stride * PixelSize;
       
        float2 horizons = SearchForLargestAngleDual(STEP_COUNT, input.TexCoords, direction, StepRadius, Offset, viewPos, normalize(-viewPos), 0.9);

        ao += ComputeInnerIntegral(horizons, direction, normalize(-viewPos), normal);
      //  randomVec.xy += direction;
      
        //Offset = frac(Offset + 0.617);
 
		// Rotate for the next angle
        
 */
        float radian = stepRadian * (i + randomVec.x);
        float sinr, cosr;
        sincos(radian, sinr, cosr);
        float2 direction = float2(cosr, sinr);
       
       
        float rayPixels = frac(randomVec.y) * stride + 1.0;
        float topOcclusion = 0.2;
        float occu = 0;
     
        for (int s = 0; s < STEP_COUNT; s++)
        {
            float2 uv2 = round(rayPixels * direction) * PixelSize + input.TexCoords;
         
            float linearDepth2 = tex2D(gProjectionDepth, uv2).x;
            float3 vpos2 = mul(float4(ReconstructViewPos(uv2, linearDepth2).xyz + CameraPos, 1), View).xyz;
         
             
            occu += ComputeAO(viewPos, vpos2, normal, topOcclusion);
            rayPixels += stride;
           
        }
      //  occu /= STEP_COUNT;
         
        ao += occu;
    }
   // ao = pow(ao * rcp((float) STEP_COUNT * DIRECTION_COUNT) * 1, 0.6);
  // ao /= (DIRECTION_COUNT);
   
    ao = ao / ((float) DIRECTION_COUNT);
    ao = 1 - ao;
//    ao *= 2.0 / PI;
    return float4(ao.xxx, 1);
  /*      float3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    float3 bitangent = cross(normal, tangent);
    float3x3 TBN = float3x3(tangent, bitangent, normal);
    
    float occlusion = 0;
    
    for (int i = 0; i < 8; i++)
    {
        float3 samplePos = mul(float3(tex2D(texNoise, input.TexCoords + float2(i / 10.0, i / 10.0)).r * 2 - 1, tex2D(texNoise, input.TexCoords + float2(0.5, 0.5) - float2(i / 10.0, i / 10.0)).g * 2 - 1, tex2D(texNoise, input.TexCoords - float2(0.8, 0.8) - float2(i / 10.0, i / 10.0)).b)*(i/8.0), TBN);
  
        samplePos = worldPos+samplePos*0.3;
    float4 sampleDepthView = mul(float4(samplePos,1), View);
    
    float4 offset = float4(samplePos, 1.0);
        offset= mul(offset, ViewProjection);
        offset.xyz /= offset.w;
        offset.xy = offset.xy * 0.5 + 0.5 ;
    offset.y = 1 - offset.y;
 //   sampleDepthView.z = sampleDepthView.z+sampleDepthView.x * 0.000000000000000000000000000000001 + sampleDepthView.y * 0.000000000000000000000000000000000001;
        
   //     float3 worldPosProj = tex2D(gPositionWS, offset.xy).xyz;
    //   float4 sampleViewPosDepth = mul(float4(worldPosProj, 1), View);
     //   float3 sampleViewPosDepth = tex2D(gPositionWS, offset.xy).xyz;
      
  //      sampleViewPosDepth.z = sampleViewPosDepth.z + sampleViewPosDepth.x * 0.000000000000000000000000000000001 + sampleViewPosDepth.y * 0.000000000000000000000000000000000001;
        float sampleDepth =/* !usingDepthReconstructWorldPos? - sampleViewPosDepth.z:(tex2D(gProjectionDepth, offset.xy).x);
        
      
   // return float4(sampleDepth.xxx, 1);
        if (sampleDepth < -sampleDepthView.z-0.03 && abs(sampleDepth - (-sampleDepthView.z)) < 0.3)
        {
            occlusion += 0;

        }
        else
        {
            occlusion += 1;
        }
         
    }
       
     
        
    occlusion /= 8.0;
    
    
    return float4(occlusion.xxx, 1);*/

}
/*



matrix projection;
matrix invProjection;

sampler2D gTangent = sampler_state
{
    Texture = <TangentTex>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
sampler2D texNoise = sampler_state
{
    Texture = <NoiseTex>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};


matrix invertView;
float2 noiseScale = float2(800.0 / 4.0, 600.0 / 4.0);
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
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = input.Position;
    output.TexCoords = float2(input.TexCoords.x,input.TexCoords.y);

    return output;
}
 
float3 PositionFromDepth(float2 vTexCoord)
{
    // Get the depth value for this pixel
    float z = tex2D(gProjectionDepth, vTexCoord).r;
    // Get x/w and y/w from the viewport position
    float x = vTexCoord.x * 2 - 1;
    float y = (1 - vTexCoord.y) * 2 - 1;
    float4 vProjectedPos = float4(x, y, z, 1.0f);
    // Transform by the inverse projection matrix
    float4 vPositionVS = mul(vProjectedPos, invProjection);
    // Divide by w to get the view-space position
    return vPositionVS.xyz/vPositionVS.w;
}
 
float doAmbientOcclusion1(in float2 tcoord, in float3 p, in float3 cnorm)
{
    float3 diff = PositionFromDepth(tcoord) - p;
    float3 v = normalize(diff);
    float d = length(diff) * 1;

    return max(0.0, dot(cnorm, v) - 0) * (1.0 / (1.0 + d)) * 1;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 p = PositionFromDepth(input.TexCoords);
    float rad = 0.3 / p.z;
   
    const float2 vec[4] =
    {
        float2(1, 0),
		float2(-1, 0),
		float2(0, 1),
		float2(0, -1)
    };
    float3 normal = tex2D(gNormal, input.TexCoords) * 2 - 1;
     
   // float3 fragPos = mul(projFragPos, invProjection);
    //fragPos = VSPositionFromDepth(input.TexCoords);
    float occlusion = 0.0;
 
    for (int i = 0; i < 64;i++)
    {
        float2 sample = samples[i].xy;
        
        occlusion += doAmbientOcclusion1(input.TexCoords + vec[i%4]*rad+samples[i]*0.1, p, normal);
         
    }
    
    
    occlusion =  (occlusion /64);
    return float4(occlusion, occlusion, occlusion, 1);

}

 


float4x4 param_inverseViewProjectionMatrix;
float4x4 param_inverseViewMatrix;
float3 param_frustumCornersVS[4];
float4x4 g_matInvProjection;
 float param_randomSize=1;
 float param_sampleRadius=1;
 float param_intensity=1;
 float param_scale=1;
float param_bias;
float2 param_screenSize;
float3x3 transposeInverseView;

texture param_normalMap;
texture param_depthMap;
texture param_randomMap;



sampler normalSampler = sampler_state
{
    Texture = (param_normalMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = NONE;
};
sampler depthSampler = sampler_state
{
    Texture = (param_depthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    MipFilter = NONE;
};
sampler randomSampler = sampler_state
{
    Texture = (param_randomMap);
    AddressU = WRAP;
    AddressV = WRAP;
    MagFilter = POINT;
    MinFilter = POINT;
    MipFilter = NONE;
};



// Define VS input
struct VSIn
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

// Define VS output and therefor PS input
struct VSOut
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
  
};

// Define PS output
struct PSOut
{
    float4 Color : COLOR0;
};



// Reconstruct view-space position from the depth buffer
float3 getPosition(in float2 vTexCoord, in float3 in_vFrustumCornerVS)
{
    float fPixelDepth = tex2D(depthSampler, vTexCoord).r;
    return float3(fPixelDepth * in_vFrustumCornerVS);
}

float3 getPosition(float2 vTexCoord)
{
    // Get the depth value for this pixel
    float z = tex2D(depthSampler, vTexCoord);
    // Get x/w and y/w from the viewport position
    float x = vTexCoord.x * 2 - 1;
    float y = (1 - vTexCoord.y) * 2 - 1;
    float4 vProjectedPos = float4(x, y, z, 1.0f);
    // Transform by the inverse projection matrix
    float4 vPositionVS = mul(vProjectedPos, g_matInvProjection);
    // Divide by w to get the view-space position
    return vPositionVS.xyz / vPositionVS.w;
}

// Calculate the occlusion term
float doAmbientOcclusion(in float2 tcoord, in float3 p, in float3 cnorm)
{
    float3 diff = getPosition(tcoord ) - p;
    float3 v = normalize(diff);
    float d = length(diff) * param_scale;

    return max(0.0, dot(cnorm, v) - param_bias) * (1.0 / (1.0 + d)) * param_intensity;
}


 
VSOut MainVS(VSIn input)
{
    VSOut output;

    output.Position = float4(input.Position, 1);
    output.TexCoord = input.TexCoord.xy;
    

    return output;
}


 
PSOut MainPS(VSOut input)
{
    PSOut output;

    const float2 vec[4] =
    {
        float2(1, 0),
		float2(-1, 0),
		float2(0, 1),
		float2(0, -1)
    };

    float3 p = getPosition(input.TexCoord );
    float3 n = normalize(tex2D(normalSampler, input.TexCoord).xyz * 2.0f - 1.0f);
    n = mul(n, transposeInverseView);
    float2 rand = normalize(tex2D(randomSampler, param_screenSize * input.TexCoord / param_randomSize).xy * 2.0f - 1.0f);

    float ao = 0.0f;
    float rad = param_sampleRadius/ p.z;

    int numIterations =4;
    for (int j = 0; j < numIterations; ++j)
    {
        float2 coord1 = reflect(vec[j], rand) * rad;
        float2 coord2 = float2(coord1.x - coord1.y, coord1.x + coord1.y) * 0.707f;

        ao += doAmbientOcclusion(input.TexCoord + coord1 * 0.25, p, n);
        ao += doAmbientOcclusion(input.TexCoord + coord2 * 0.50, p, n);
        ao += doAmbientOcclusion(input.TexCoord + coord1 * 0.75, p, n);
        ao += doAmbientOcclusion(input.TexCoord + coord2 * 1.00, p, n);
    }

    ao /= (float) numIterations ;
    ao = saturate(ao * param_intensity);

    output.Color = 1 - ao;

    return output;
}

*/

technique Default
{
    pass SSAO
    {
        VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
    }
}