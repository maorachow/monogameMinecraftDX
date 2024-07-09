#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif


sampler2D copyTexture = sampler_state
{
    Texture = <TextureCopy>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
float3 backgroundCol;
bool useBkgColor;
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
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = input.Position;
    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	
   /* if ((tex2D(copyTexture, input.TexCoord.xy).a) < 0.0001 && useBkgColor)
    {
        
        return float4(backgroundCol.xyz, 0);
    }*/
   /* if ((tex2D(copyTexture, input.TexCoord.xy).a) < 0.0001)
    {
        discard;
    }*/
    return float4(tex2D(copyTexture, input.TexCoord.xy).xyzw);
}

technique TextureCopy
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};