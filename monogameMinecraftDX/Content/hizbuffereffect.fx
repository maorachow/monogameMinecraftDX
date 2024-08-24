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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    output.Position = input.Position;
    output.TexCoord = input.TexCoord;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 depth = float4(
        tex2D(copyTexture, input.TexCoord.xy + float2(0.5, 0.5) * PixelSize).x > 0.1 ? tex2D(copyTexture, input.TexCoord.xy + float2(0.5, 0.5) * PixelSize).x : 1000,
        tex2D(copyTexture, input.TexCoord.xy + float2(-0.5, -0.5) * PixelSize).x > 0.1 ? tex2D(copyTexture, input.TexCoord.xy + float2(-0.5, -0.5) * PixelSize).x : 1000,
        tex2D(copyTexture, input.TexCoord.xy + float2(-0.5, 0.5) * PixelSize).x > 0.1 ? tex2D(copyTexture, input.TexCoord.xy + float2(-0.5, 0.5) * PixelSize).x : 1000,
        tex2D(copyTexture, input.TexCoord.xy + float2(0.5, -0.5) * PixelSize).x > 0.1 ? tex2D(copyTexture, input.TexCoord.xy + float2(0.5, -0.5) * PixelSize).x : 1000);
   
    return float4(min(min(depth.x, depth.y), min(depth.z, depth.w)).x, 0, 0, 1);
}

technique HiZBuffer
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};