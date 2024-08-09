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

sampler2D tex = sampler_state
{
    Texture = <Texture>;
 
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
    uint vertID : SV_VertexID;
};
struct VertexShaderInstancingInput
{
   

    float4 matRow0 : TEXCOORD2;
    
    float4 matRow1 : TEXCOORD3;
    
    float4 matRow2 : TEXCOORD4;
    
    float4 matRow3 : TEXCOORD5;
    float4 uvWidthCorner : TEXCOORD6;
    float scale : TEXCOORD7;
};
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
    
};

VertexShaderOutput MainVS(in VertexShaderInput input, VertexShaderInstancingInput inputInstancing)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    matrix worldInstancing = float4x4(inputInstancing.matRow0, inputInstancing.matRow1, inputInstancing.matRow2, inputInstancing.matRow3);
    matrix worldView = mul(worldInstancing, View);
	
    worldView[0][0] = 1.0 * inputInstancing.scale;
      worldView[0][1] = 0.0;
    worldView[0][2] = 0.0;

    
    worldView[1][0] = 0.0;
    worldView[1][1] = 1.0 * inputInstancing.scale;
    worldView[1][2] = 0.0;
   
    worldView[2][0] = 0.0;
    worldView[2][1] = 0.0;
    worldView[2][2] = 1.0*inputInstancing.scale;
   float4 worldViewPos = mul(input.Position, worldView);
    output.Position = mul(worldViewPos, Projection);
    output.TexCoord = input.TexCoord;
    switch (input.vertID)
    {
        case 0:
            output.TexCoord.xy =inputInstancing. uvWidthCorner.xy;
            break;
        case 1:
            output.TexCoord.xy = inputInstancing.uvWidthCorner.xy + float2(inputInstancing.uvWidthCorner.z,0);
            break;
        case 2:
            output.TexCoord.xy = inputInstancing.uvWidthCorner.xy + inputInstancing.uvWidthCorner.zw;;
          break;
        case 3:
            output.TexCoord.xy = inputInstancing.uvWidthCorner.xy + float2(0, inputInstancing.uvWidthCorner.w);
            break;
    }
 
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    return float4(tex2D(tex,input.TexCoord).xyz, 1);
}

technique BillboardTest
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};