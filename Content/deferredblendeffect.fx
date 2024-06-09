#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

 
sampler2D deferredLumSampler = sampler_state
{
    Texture = <TextureDeferredLum>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
sampler2D albedoSampler = sampler_state
{
    Texture = <TextureAlbedo>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Border;
    AddressV = Border;
};
sampler2D reflectionSampler = sampler_state
{
    Texture = <TextureReflection>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D ssidSampler = sampler_state
{
    Texture = <TextureIndirectDiffuse>;
 
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D aoSampler = sampler_state
{
    Texture = <TextureAO>;
 
    MipFilter = Linear;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};
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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = input.Position;
    output.TexCoords = input.TexCoords;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    if (any(tex2D(albedoSampler, input.TexCoords).xyz) < 0.02)
    {
        discard;
    }
    float3 color = tex2D(deferredLumSampler, input.TexCoords).xyz;
    float3 reflection = tex2D(reflectionSampler, input.TexCoords).xyz;
    float3 indirectDiffuse = tex2D(ssidSampler, input.TexCoords).xyz;
    float3 ambient = (indirectDiffuse + reflection) * tex2D(aoSampler,input.TexCoords).x;
    float3 final = color + ambient;
    final = final / (final + float3(1.0, 1.0, 1.0));
    final = pow(final, float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));
	return float4(final.xyz,1);
}

technique DeferredBlend
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};