#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
 

float4x4 World;
float4x4 View;
float4x4 Projection;

float3x3 NormalMat;

sampler2D textureSampler = sampler_state
{
    Texture = <TextureE>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};
float3 DiffuseColor = float3(1, 1, 1);


struct VertexShaderInput
{
    
 
	float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float4 PositionV : TEXCOORD2;
    float4 PositionWS : TEXCOORD3;
    float4 Normal : TEXCOORD1;
    float2 TexureCoordinate : TEXCOORD0;
};
struct PixelShaderOutput
{
    float4 ProjectionDepth : COLOR0;
  //  float4 Normal : COLOR2;
    
    float4 NormalWS : COLOR1;
    float4 Albedo : COLOR2;
    float4 MetallicEmissionRoughness : COLOR3;
	
};
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.PositionV = viewPosition;
    output.PositionWS = worldPosition;
	output.TexureCoordinate = input.TexureCoordinate;
    output.Normal = 
   float4((mul(float4(input.Normal.xyz, 1), float4x4(float4(NormalMat._11, NormalMat._12, NormalMat._13,1),
    float4(NormalMat._21, NormalMat._22, NormalMat._23, 1),
    float4(NormalMat._31, NormalMat._32, NormalMat._33, 1),
    float4(0, 0, 0, 1))).xyz * 0.5 + 0.5).xyz, 1);
	return output;
}

PixelShaderOutput MainPS(VertexShaderOutput input) : COLOR
{
    PixelShaderOutput psOut = (PixelShaderOutput) 0;
    psOut.ProjectionDepth = float4((-input.PositionV.z).x,0,0,1);
    psOut.NormalWS = float4(input.Normal.xyz, 1);
    psOut.Albedo = float4(((tex2D(textureSampler, input.TexureCoordinate).xyz) * DiffuseColor).xyz, 1);
    psOut.MetallicEmissionRoughness =float4(0.1,0,0.1,1);
    return psOut;
}

technique GBufferEntity1
{
	pass 
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};