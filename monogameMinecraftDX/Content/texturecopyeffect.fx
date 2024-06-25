#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif


float2 pixelSize;
sampler2D copyTexture = sampler_state
{
    Texture = <TextureCopy>;
 
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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position =input.Position;
	output.TexCoord = input.TexCoord;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	
    float2 texCoords = input.TexCoord;
    float2 texCoords1 = input.TexCoord + float2(-1.5, -1.5) * pixelSize;
    float2 texCoords2 = input.TexCoord + float2(1.5, 1.5) * pixelSize;
    float2 texCoords3 = input.TexCoord + float2(1.5, -1.5) * pixelSize;
    float2 texCoords4 = input.TexCoord + float2(-1.5, 1.5) * pixelSize;
    float2 texCoordsOrigin = input.TexCoord * 16;
   
    float2 ceilTexCoords = float2(floor(texCoordsOrigin.x) + 1, floor(texCoordsOrigin.y) + 1);
    float2 floorTexCoords = float2(floor(texCoordsOrigin.x), floor(texCoordsOrigin.y));
    float2 texCoordLimitBottom = floorTexCoords / 16;
    float2 texCoordLimitUp = ceilTexCoords / 16;
    texCoords1.x = clamp(texCoords1.x, texCoordLimitBottom.x, texCoordLimitUp.x - 0.0002);
    texCoords1.y = clamp(texCoords1.y, texCoordLimitBottom.y, texCoordLimitUp.y - 0.0002);
    texCoords2.x = clamp(texCoords2.x, texCoordLimitBottom.x, texCoordLimitUp.x - 0.0002);
    texCoords2.y = clamp(texCoords2.y, texCoordLimitBottom.y, texCoordLimitUp.y - 0.0002);
    texCoords3.x = clamp(texCoords3.x, texCoordLimitBottom.x, texCoordLimitUp.x - 0.0002);
    texCoords3.y = clamp(texCoords3.y, texCoordLimitBottom.y, texCoordLimitUp.y - 0.0002);
    texCoords4.x = clamp(texCoords4.x, texCoordLimitBottom.x, texCoordLimitUp.x - 0.0002);
    texCoords4.y = clamp(texCoords4.y, texCoordLimitBottom.y, texCoordLimitUp.y - 0.0002);
    int coloredSampleCount = 0;
    if (length(tex2D(copyTexture, texCoords1.xy).xyz)>0.001)
    {
        coloredSampleCount++;

    }
    if (length(tex2D(copyTexture, texCoords2.xy).xyz) > 0.001)
    {
        coloredSampleCount++;

    }
    if (length(tex2D(copyTexture, texCoords3.xy).xyz) > 0.001)
    {
        coloredSampleCount++;

    }
    if (length(tex2D(copyTexture, texCoords4.xy).xyz) > 0.001)
    {
        coloredSampleCount++;

    }
    float4 finalColor = float4(tex2D(copyTexture, texCoords1.xy).xyz + tex2D(copyTexture, texCoords2.xy).xyz + tex2D(copyTexture, texCoords3.xy).xyz + tex2D(copyTexture, texCoords4.xy).xyz,tex2D(copyTexture, texCoords1.xy).w+tex2D(copyTexture, texCoords2.xy).w+tex2D(copyTexture, texCoords3.xy).w+tex2D(copyTexture, texCoords4.xy).w);
    if (coloredSampleCount <= 0)
    {
        finalColor.xyzw = 0;

    }
    finalColor.xyzw /= clamp(coloredSampleCount, 1, 4);
    if (length(finalColor.xyz) < 0.001)
    {
        finalColor.a = 0;
    }
        return finalColor;
}

technique TextureCopy
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};