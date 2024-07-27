#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif


matrix View;
matrix Projection;
sampler2D DepthSampler = sampler_state
{
    Texture = <TextureDepth>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float viewDepth : TEXCOORD1;
    float4 PositionP : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 positionV = mul(input.Position, View);
    output.Position = mul(positionV, Projection);
	output.TexCoord = input.TexCoord;
    output.viewDepth =-positionV.z;
    output.PositionP = mul(positionV, Projection);
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
   
        return float4(tex2D(TextureSampler, input.TexCoord).xyz, 1);
}

technique DebugLineDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};