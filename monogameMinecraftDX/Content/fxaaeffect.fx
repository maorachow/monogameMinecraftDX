#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif


sampler2D inputSampler = sampler_state
{
    Texture = <InputTexture>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Border;
    AddressV = Border;
};
float2 PixelSize;
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


float rgb2luma(float3 rgb)
{
    return (dot(rgb, float3(0.299, 0.587, 0.114)));
}
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position =input.Position;
	output.TexCoord = input.TexCoord;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 M = tex2D(inputSampler, input.TexCoord).xyz;
    float3 NW = tex2D(inputSampler, input.TexCoord + PixelSize * float2(-0.5, 0.5)).xyz;

    float3 SW = tex2D(inputSampler, input.TexCoord + PixelSize * float2(-0.5, -0.5)).xyz;

    float3 SE = tex2D(inputSampler, input.TexCoord + PixelSize * float2(0.5, -0.5)).xyz;

    float3 NE = tex2D(inputSampler, input.TexCoord + PixelSize * float2(0.5, 0.5)).xyz;
//    return float4((NW + NE + SW + SE) / 4.0, 1);
    
    float ML = rgb2luma(M);
    float NWL = rgb2luma(NW);

    float SWL = rgb2luma(SW);

    float SEL = rgb2luma(SE);

    float NEL = rgb2luma(NE);
   
    float MaxLuma = max(max(max(NWL, NEL), max(SWL, SEL)), ML);
    float MinLuma = min(min(min(NWL, NEL), min(SWL, SEL)), ML);
    float Contrast = MaxLuma - MinLuma;
    if (Contrast <= max(0.0312, MaxLuma * 0.125))
    {
        return float4(M,1);
    }
  //  return float4(((NWL + SWL + SEL + NEL) / 4.0).xxx, 1);
    NEL += 1.0f / 384.0f;
    float2 Dir;
    Dir.x = -((NWL + NEL) - (SWL + SEL));
    Dir.y = ((NEL + SEL) - (NWL + SWL));
    Dir = normalize(Dir);
    float2 Dir1 = Dir * PixelSize.xy * 0.5;
    float4 N1 = tex2D(inputSampler, input.TexCoord + Dir1);
    float4 P1 = tex2D(inputSampler, input.TexCoord - Dir1);
    float4 Result =(N1 + P1)*0.5;
    
    
    float DirAbsMinTimesC = min(abs(Dir.x), abs(Dir.y))*8;
     float2 Dir2 = clamp(Dir / DirAbsMinTimesC, -4, 4) * 2;
    
    
    float4 N2 = tex2D(inputSampler, input.TexCoord + Dir2*PixelSize.xy );
    float4 P2 = tex2D(inputSampler, input.TexCoord - Dir2*PixelSize.xy );
    float4 Result2 = Result * 0.5f + (N2 + P2) * 0.25f;

    if (rgb2luma(Result2.xyz) > MinLuma && rgb2luma(Result2.xyz) < MaxLuma)
    {
        Result = Result2;
    }
    return Result;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};