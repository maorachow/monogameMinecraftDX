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

sampler2D texMER = sampler_state
{
    Texture = <TextureMER>;
 
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler2D texNormal = sampler_state
{
    Texture = <TextureNormal>;
 
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
    
    float3 Normal : NORMAL0;
    float3 Tangent : TANGENT0;
    uint2 VertID : TEXCOORD8;
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
    float4 PositionV : TEXCOORD2;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3x3 TBN : TEXCOORD3;
    
};

VertexShaderOutput MainVS(in VertexShaderInput input, VertexShaderInstancingInput inputInstancing)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;

    matrix worldInstancing = float4x4(inputInstancing.matRow0, inputInstancing.matRow1, inputInstancing.matRow2, inputInstancing.matRow3);
    matrix worldView = mul(worldInstancing, View);
    float3x3 normalMat = float3x3(worldInstancing[0][0], worldInstancing[0][1], worldInstancing[0][2]
    
    , worldInstancing[1][0], worldInstancing[1][1], worldInstancing[1][2],
    worldInstancing[2][0], worldInstancing[2][1], worldInstancing[2][2]);
  /*  worldView[0][0] = 1.0 * inputInstancing.scale;
    worldView[0][1] = 0.0;
    worldView[0][2] = 0.0;

    
    worldView[1][0] = 0.0;
    worldView[1][1] = 1.0 * inputInstancing.scale;
    worldView[1][2] = 0.0;
   
    worldView[2][0] = 0.0;
    worldView[2][1] = 0.0;
    worldView[2][2] = 1.0 * inputInstancing.scale;*/
    float4 worldViewPos = mul(input.Position, worldView);
    output.PositionV = worldViewPos;
    output.Position = mul(worldViewPos, Projection);
    output.TexCoord = input.TexCoord;
    float3 normalWS = normalize(mul(input.Normal, normalMat));
    float3 tangentWS = normalize(mul(-input.Tangent, normalMat));
    float3 bitTangentWS = cross(normalWS, tangentWS);
    float3x3 TBN = float3x3(tangentWS, bitTangentWS, normalWS);
    output.TBN = TBN;
    output.Normal = normalize(mul(input.Normal, normalMat));
    switch (input.VertID.x)
    {
        case 0:
            output.TexCoord.xy = inputInstancing.uvWidthCorner.xy;
            break;
        case 1:
            output.TexCoord.xy = inputInstancing.uvWidthCorner.xy + float2(inputInstancing.uvWidthCorner.z, 0);
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

 
struct PixelShaderOutput
{
    float4 ProjectionDepth : COLOR0;
  //  float4 Normal : COLOR2;
    
    float4 NormalWS : COLOR1;
    float4 Albedo : COLOR2;
    float4 MetallicEmissionRoughness : COLOR3;
	
};
PixelShaderOutput MainPS(VertexShaderOutput input) : COLOR
{
    if (tex2D(tex, input.TexCoord).a < 0.1)
    {
        discard;
    }
    PixelShaderOutput psOut = (PixelShaderOutput) 0;
    
    psOut.ProjectionDepth = float4((-input.PositionV.z).x, 0, 0, 1);
    
    float3 normal = mul(abs(tex2D(texNormal, input.TexCoord).xyz * 2 - 1).x < 0.99 || abs(tex2D(texNormal, input.TexCoord).xyz * 2 - 1).y < 0.99 || abs(tex2D(texNormal, input.TexCoord).xyz * 2 - 1).z < 0.99 ? tex2D(texNormal, input.TexCoord).xyz * 2 - 1 : float3(0, 0, 1), input.TBN);
    if (length(normal) < 0.001)
    {
        normal = mul(float3(0.5, 0.5, 1), input.TBN);
    }
    psOut.NormalWS = float4(normal * 0.5f + 0.5f, 1);
    psOut.Albedo = float4(tex2D(tex, input.TexCoord).xyz, 1);
    psOut.MetallicEmissionRoughness = float4(tex2D(texMER,input.TexCoord).xyz, 1);
    return psOut;
}

technique GBufferParticle
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};