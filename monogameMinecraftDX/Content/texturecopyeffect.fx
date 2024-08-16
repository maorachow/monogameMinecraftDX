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
 
    Filter = Point;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
    AddressW = Clamp;

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
    float2 texCoords1 = input.TexCoord + float2(-1.1, -1.1) * pixelSize;
    float2 texCoords2 = input.TexCoord + float2(1.1, 1.1) * pixelSize;
    float2 texCoords3 = input.TexCoord + float2(1.1, -1.1) * pixelSize;
    float2 texCoords4 = input.TexCoord + float2(-1.1, 1.1) * pixelSize;
    float2 texCoordsOrigin = input.TexCoord * 16.0;
   
    float2 ceilTexCoords = float2(floor(texCoordsOrigin.x) + 1, floor(texCoordsOrigin.y) + 1);
    float2 floorTexCoords = float2(floor(texCoordsOrigin.x), floor(texCoordsOrigin.y));
    float2 texCoordLimitBottom = floorTexCoords / 16.0;
    float2 texCoordLimitUp = ceilTexCoords / 16.0;
    texCoords1.x = clamp(texCoords1.x, texCoordLimitBottom.x, texCoordLimitUp.x - 0.00002);
    texCoords1.y = clamp(texCoords1.y, texCoordLimitBottom.y, texCoordLimitUp.y - 0.00002);
    texCoords2.x = clamp(texCoords2.x, texCoordLimitBottom.x, texCoordLimitUp.x - 0.00002);
    texCoords2.y = clamp(texCoords2.y, texCoordLimitBottom.y, texCoordLimitUp.y - 0.00002);
    texCoords3.x = clamp(texCoords3.x, texCoordLimitBottom.x, texCoordLimitUp.x - 0.00002);
    texCoords3.y = clamp(texCoords3.y, texCoordLimitBottom.y, texCoordLimitUp.y - 0.00002);
    texCoords4.x = clamp(texCoords4.x, texCoordLimitBottom.x, texCoordLimitUp.x - 0.00002);
    texCoords4.y = clamp(texCoords4.y, texCoordLimitBottom.y, texCoordLimitUp.y - 0.00002);
    int coloredSampleCount = 0;
   
    float4 color1 = tex2D(copyTexture, texCoords1.xy);
    float4 color2 = tex2D(copyTexture, texCoords2.xy);
    float4 color3 = tex2D(copyTexture, texCoords3.xy);
    float4 color4 = tex2D(copyTexture, texCoords4.xy);
    if (color1.w> 0.001)
    {
        coloredSampleCount++;

    }
    if (color2.w> 0.001)
    {
        coloredSampleCount++;

    }
    if (color3.w>0.001)
    {
        coloredSampleCount++;

    }
    if (color4.w>0.001)
    {
        coloredSampleCount++;

    }
    float4 finalColor = float4(color1.xyz + color2.xyz + color3.xyz + color4.xyz, color1.a + color2.a + color3.a + color4.a);
  //  float4 finalColor = float4(color1.xyz , color1.a);
    
    if (coloredSampleCount <= 0)
    {
        finalColor.xyzw = 0;
        return finalColor;
    }
    finalColor.xyzw /= coloredSampleCount;
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