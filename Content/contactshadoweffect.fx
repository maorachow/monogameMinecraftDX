 
 
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
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = NONE;
};
matrix View;
matrix ViewProjection;
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
    float noiseValue = tex2D(noiseTex, input.TexCoords*10).r;
    
    bool isHit = false;
    [unroll(8)]
    for (int i = 0; i < 8; i++)
    {
        float3 marchPos = rayOrigin + marchDir * (0.16 * (i+0.5 + noiseValue) );
       
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
       if (sampleViewDepth < testDepth && abs(sampleViewDepth-testDepth)<0.2)
        {
            isHit = true;
           
            break; //   return float4(0, 0, 0, 1);
        }
        
        
    }
	
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