 
 
sampler2D gPositionWS = sampler_state
{
    Texture = <PositionWSTex>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D gNormal = sampler_state
{
    Texture = <NormalTex>;
 
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
sampler gProjectionDepth = sampler_state
{
    Texture = (ProjectionDepthTex);
    AddressU = Border;
    AddressV = Border;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = NONE;
};
matrix View;
matrix Projection;
matrix ViewProjection;
float2 PixelSize;
matrix ViewOrigin;
float3 LightDir;
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

 

 
float4 ProjectionParams2;
float4 CameraViewTopLeftCorner;
float4 CameraViewXExtent;
float4 CameraViewYExtent;

float3 CameraPos;


float4 TransformViewToHScreen(float3 vpos, float2 screenSize)
{
    float4 cpos = mul(float4(vpos, 1), Projection);
    cpos.xy = float2(cpos.x, -cpos.y) * 0.5 + 0.5 * cpos.w; //
    cpos.xy *= screenSize;
    return cpos;
}

float3 ReconstructViewPos(float2 uv, float linearEyeDepth)
{
  //  uv.y = 1.0 - uv.y;
    float zScale = linearEyeDepth * ProjectionParams2.x; // divide by near plane  
    float3 viewPos = CameraViewTopLeftCorner.xyz + CameraViewXExtent.xyz * uv.x + CameraViewYExtent.xyz * uv.y;
    viewPos *= zScale;
    return viewPos;
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
       
    marchDepthView.z = marchDepthView.z ;
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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = input.Position;
    output.TexCoords = input.TexCoords;

	return output;
}
void swap(inout float v0, inout float v1)
{
    float temp = v0;
    v0 = v1;
    v1 = temp;
}
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 worldPos = ReconstructViewPos(input.TexCoords,tex2D(gProjectionDepth, input.TexCoords).x)+CameraPos;
    float3 normal = tex2D(gNormal, input.TexCoords) * 2 - 1;
    worldPos = worldPos + normal * 0.2 * length(worldPos - CameraPos) / 150;
    float3 marchDir = normalize(LightDir);
    if (marchDir.y < 0.1)
    {
        return float4(0, 0, 0, 1);
    }
    float viewDepth = tex2D(gProjectionDepth, input.TexCoords).x;
    if (viewDepth > 40||viewDepth<0.001)
    {
        return float4(1, 1, 1, 1);
    }
    float3 rayOrigin = worldPos;
    float noiseValue = tex2D(noiseTex, input.TexCoords*10).r+0.5;
    
    
    
    
    float maxDist = 40.0f;
   

    float3 viewRDir = normalize(mul(float4(marchDir, 1), ViewOrigin).xyz);
    
   // return float4(viewRDir.xyz, 1);
    float3 viewPosOrigin = mul(float4(rayOrigin, 1), View);
   
   
    float end = viewPosOrigin.z + viewRDir.z * maxDist;
    if (end > -0.1)
    {
     //   return float4(0, 1, 0, 1);
        maxDist = abs(-0.1 - viewPosOrigin.z) / viewRDir.z;
    }
    
    float3 viewPosEnd = viewPosOrigin + viewRDir * maxDist;
 //   float4 projPos = mul(float4(viewPosEnd, 1), Projection);
   // return float4((projPos.xy / projPos.w) * 0.5 + 0.5, 0, 1);
    float4 startHScreen = TransformViewToHScreen(viewPosOrigin, 1.0 / (PixelSize.xy));
    float4 endHScreen = TransformViewToHScreen(viewPosEnd, 1.0 / (PixelSize.xy));
  // return float4((startHScreen.xy / startHScreen.w) * PixelSize, 0, 1);
    float startK = 1.0 / startHScreen.w;
    float endK = 1.0 / endHScreen.w;
    float2 startScreen = startHScreen.xy * startK;
    float2 endScreen = endHScreen.xy * endK;
    
    float3 startQ = viewPosOrigin * startK;

    float3 endQ = viewPosEnd * endK;
    
    
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
    dp *= (30 + noiseValue*8) / (-viewPosOrigin.z);
    dq *= (30 + noiseValue * 8) / (-viewPosOrigin.z);
    dk *= (30 + noiseValue * 8) / (-viewPosOrigin.z);
    
    float rayZMin = viewPosOrigin.z;
    float rayZMax = viewPosOrigin.z;
    float preZ = viewPosOrigin.z;

    float2 P = startScreen;
    float3 Q = startQ;
    float K = startK;
    
    
    bool isHit = false;
    int marchingSteps = 0;
     [loop]
    for (int i = 0; i < 18; i++)
    {
        marchingSteps++;
        float2 marchPos = P +dp;
      /*  if (length(marchPos - P) < 1.0)
        {
            return float4(1, 1, 0, 1);
        }*/
        P += dp;
        Q += dq;
        K += dk;
        
      
        rayZMax = (Q.z) / ( K);
       
        float2 hitUV = permute ? P.yx : P;
        hitUV *= PixelSize;
        if (hitUV.x < 0 || hitUV.x > 1 || hitUV.y < 0 || hitUV.y > 1)
        {
            isHit = false;
           
            break; // return float4(1, 1, 1, 1);
        }
        float sampleDepthM0 = -tex2D(gProjectionDepth, hitUV.xy).x;

    
      
        
        float surfaceDepth = sampleDepthM0;
        if (-surfaceDepth / 1000 > 0.99 || (surfaceDepth)> 0  || (rayZMax)>0)
        {
          
            isHit = false;
            
            break;
        }
        
        bool isBehind = (rayZMax < surfaceDepth);
        
     //   float avgTestDepth = (rayZMin + rayZMax) / 2.0f;
        bool intersecting = isBehind && (abs(rayZMax - surfaceDepth) < 0.1);

        if (intersecting)
        {
            isHit = true;
           
            break; 
        }

    }
  /*  [loop]
    for (int i = 0; i < 8; i++)
    {
        float3 marchPos = rayOrigin + marchDir * (0.1 * (i+0.1 + noiseValue) );
       
        float2 uv = GetScreenCoordFromWorldPos(marchPos);
    //    return float4(uv.xy,1, 1);
    //    float3 sampleWorldPos = tex2D(gPositionWS, uv).xyz;
       
        float sampleViewDepth = tex2D(gProjectionDepth, uv).x;
        //GetViewDepthFromWorldPos(sampleWorldPos); 
        float testDepth = GetViewDepthFromWorldPos(marchPos);
       if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1||sampleViewDepth<0.01)
        {
            isHit = false;
           
            break; // return float4(1, 1, 1, 1);
        }
       if (sampleViewDepth < testDepth && abs(sampleViewDepth-testDepth)<0.12)
        {
            isHit = true;
           
            break; //   return float4(0, 0, 0, 1);
        }
        
        
    }*/
	
    if (isHit == true)
    {
        
        return float4(0, 0, 0, 1);
    }
    else
    {
        return float4(1, 1, 1, 1);
    }
     
    
}

technique ContactShadow
{
	pass P0
	{
        VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
    }
};