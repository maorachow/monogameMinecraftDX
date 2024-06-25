#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif


sampler2D inputTexture = sampler_state
{
    Texture = <InputTexture>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D motionVectorTex = sampler_state
{
    Texture = <MotionVectorTex>;
 
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
  //  
    if (tex2D(motionVectorTex, input.TexCoord).z > 0.8f)
    {
        return float4(tex2D(inputTexture, input.TexCoord.xy).xyz, 1);
    }
    float2 prevTexCoord = input.TexCoord + tex2D(motionVectorTex, input.TexCoord).xy;
    float2 delta = prevTexCoord-input.TexCoord  ;
   
    float deltaLength = length(delta);
    float minDist = length(PixelSize * 5.0);
    float maxDist = length(PixelSize * 15.0);
    
    if (deltaLength > maxDist)
    {
        delta = normalize(delta) * maxDist;

    }
    [branch]
    if (deltaLength < minDist)
    {
        return float4(tex2D(inputTexture, input.TexCoord.xy).xyz, 1);

    }
      
   // delta = clamp(delta, normalize(delta) * -maxDist, normalize(delta) * maxDist);

    
    float3 color = 0;
    int sampleCount = 0;
    [unroll]
    for (int i = 0; i < 5; i++)
    {
        float2 texCoord = input.TexCoord+delta*(i/5.0) ;
        if (texCoord.x < 0 || texCoord.x > 1 || texCoord.y < 0 || texCoord.y > 1)
        {
            break;
        }
        color += tex2D(inputTexture, texCoord.xy).xyz;
        sampleCount++;

    }
    color /= sampleCount;
    return float4(color.xyz, 1);
  
}

technique MotionBlur
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};