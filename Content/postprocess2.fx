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
sampler2D gProjectionDepth = sampler_state
{
    Texture = <ProjectionDepthTex>;
 
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
float3 hsv2rgb(in float3 c)
{
    float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);

    return c.z * lerp(float3(1.0, 1.0, 1.0), rgb, c.y);
}

float3 rgb2hsb(in float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}
float4 MainPS(VertexShaderOutput input) : COLOR
{
  //  
  /*  if (tex2D(motionVectorTex, input.TexCoord).z > 0.8f)
    {
        return float4(tex2D(inputTexture, input.TexCoord.xy).xyz, 1);
    }
    float2 prevTexCoord = input.TexCoord + tex2D(motionVectorTex, input.TexCoord).xy;
    float2 delta = prevTexCoord-input.TexCoord  ;
    delta *= 10;
    float deltaLength = length(delta);
    float minDist = length(PixelSize * 5.0);
    float maxDist = length(PixelSize * 75.0);
    
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
    color /= sampleCount;*/
    float dist = distance(input.TexCoord.xy, float2(0.5, 0.5));
    dist *= 1.5;
    dist = clamp(dist, 0, 1);
    float3 color = lerp(tex2D(inputTexture, input.TexCoord.xy).xyz, float3(0, 0, 0), dist);
    return float4(color.xyz, 1);
  
}

technique PostProcess2
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};