#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

matrix prevView;
matrix prevProjection;
matrix View;
matrix Projection;



sampler2D gPositionWS = sampler_state
{
    Texture = <PositionWSTex>;
 
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
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = input.Position;
	output.TexCoord = input.TexCoord;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    if (length(tex2D(gProjectionDepth, input.TexCoord).x) < 0.001)
    {
        return float4(0, 0, 1, 1);
    }
    float3 worldPos = ReconstructViewPos(input.TexCoord,tex2D(gProjectionDepth,input.TexCoord).x)+CameraPos;
   
    float4 curClipPos = float4(worldPos, 1);
    curClipPos = mul(curClipPos, View);
    curClipPos = mul(curClipPos, Projection);
    float2 curTexCoord = curClipPos.xy / curClipPos.w;
    curTexCoord = curTexCoord * 0.5 + 0.5;
    curTexCoord.y = 1 - curTexCoord.y;
    float4 prevClipPos = float4(worldPos, 1);
    prevClipPos = mul(prevClipPos, prevView);
    prevClipPos = mul(prevClipPos, prevProjection);
    float2 prevTexCoord = prevClipPos.xy / prevClipPos.w;
    prevTexCoord = prevTexCoord * 0.5 + 0.5;
    prevTexCoord.y = 1 - prevTexCoord.y;
    return float4(prevTexCoord.xy-curTexCoord.xy, 0, 1);
}

technique MotionVector
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};