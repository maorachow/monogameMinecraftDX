 

matrix World;
matrix View;
matrix Projection;
float3x3 TransposeInverseView;
 
float roughness;
 

sampler textureSampler = sampler_state
{
    Texture = (blockTex);
    AddressU = CLAMP;
    AddressV = CLAMP;
   
    Filter = Point;
    Mipfilter = Linear;
    MipLODBias = -2;
    MaxLOD = 8;
   
};

sampler normalSampler = sampler_state
{
    Texture = (normalTex);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
    MipLODBias =-2;
    MaxLOD = 8;
   
};
sampler depthSamplerOpq = sampler_state
{
    Texture = (depthTexOpq);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
  
   
};

sampler depthSampler = sampler_state
{
    Texture = (depthTex);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
  
   
};

sampler depthSampler1 = sampler_state
{
    Texture = (depthTex1);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
  
   
};
sampler merSampler = sampler_state
{
    Texture = (merTex);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = Point;
    MinFilter = Point;
    Mipfilter = Point;
  
   
};
struct VertexShaderInput
{
    float4 Position : Position;
    float3 Normal : NORMAL0;
    float2 TexureCoordinate : TEXCOORD0;
    float3 Tangent : TANGENT0;
};

struct VertexShaderOutput
{
    float4 PositionScreenSpace  : SV_Position;
    float4 PositionV : TEXCOORD1;
    float4 PositionP : TEXCOORD3;
  //  float4 PositionP : TEXCOORD4;
    float3 PositionWS : TEXCOORD5;
    float3 Normal : TEXCOORD2;
   
    float3x3 TBN : TEXCOORD6;
    float2 TexCoords : TEXCOORD11;
};
struct PixelShaderOutput
{
    
     
    float4 ProjectionDepth : COLOR0;
  //  float4 Normal : COLOR2;
    
    float4 NormalWS : COLOR1;
    float4 Albedo : COLOR2;
    float4 MetallcEmissionRoughness : COLOR3;
};

 
 
VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.PositionScreenSpace = mul(viewPosition, Projection);
    
    output.PositionV = viewPosition;
    output.PositionP = mul(viewPosition, Projection);
    float3x3 worldView =   World*View;
    
    
    
    output.Normal =input.Normal;
    
    float3 BitTangent = cross(input.Normal, input.Tangent);
    float3 T = normalize((input.Tangent));
    float3 B = normalize(BitTangent);
    float3 N = normalize(input.Normal);
    float3x3 TBN = float3x3(T, B, N);
    output.TBN = TBN;
    output.PositionWS = worldPosition.xyz/worldPosition.w;
    output.TexCoords = input.TexureCoordinate;
    
	return output;
}
 
float LinearizeDepth(float depth)
{
    float NEAR = 0.1;
     float FAR = 100.0f;
    float z = depth * 2.0 - 1.0; 
    return (2.0 * NEAR * FAR) / (FAR + NEAR - z * (FAR - NEAR));
}
PixelShaderOutput MainPS(VertexShaderOutput input) 
{
    PixelShaderOutput psOut = (PixelShaderOutput) 0;
    
      
    if (tex2D(textureSampler, input.TexCoords).a < 0.001)
    {
        discard;
    }
    
    
    
    
   
    
    
    float2 screenSpacePos = input.PositionP.xy / input.PositionP.w;
    screenSpacePos.xy = screenSpacePos.xy * 0.5 + 0.5;
    screenSpacePos.y = 1 - screenSpacePos.y;
    float sampleDepthOpq = tex2D(depthSamplerOpq, screenSpacePos).x;

    if (sampleDepthOpq < (-input.PositionV.z) && sampleDepthOpq > 0.01)
    {
        discard;
    }
    float sampleDepth = max(tex2D(depthSampler, screenSpacePos).x, tex2D(depthSampler1, screenSpacePos).x);
    if (sampleDepth > (-input.PositionV.z - 0.01) && sampleDepth > 0.01)
    {
        discard;
    }
    
    psOut.ProjectionDepth.a = 1;
   // float z = -input.PositionV.z;
  //  float packedZ = ((1 / z) - 1 / 0.1) / (1 / 500 - 1 / 0.1);
  //  float packedZ1 = z / 50.0;
    psOut.ProjectionDepth.r = -input.PositionV.z;
    float3 normal = mul(abs(tex2D(normalSampler, input.TexCoords).xyz * 2 - 1).x < 0.99 || abs(tex2D(normalSampler, input.TexCoords).xyz * 2 - 1).y < 0.99 || abs(tex2D(normalSampler, input.TexCoords).xyz * 2 - 1).z < 0.99 ? tex2D(normalSampler, input.TexCoords).xyz * 2 - 1 : float3(0, 0, 1) , input.TBN);
    if (length(normal) < 0.001 )
    {
        normal = mul(float3(0.5,0.5,1), input.TBN);
    }
    psOut.NormalWS = float4(normal*0.5+0.5, 1);
    psOut.Albedo = float4(tex2D(textureSampler,input.TexCoords).xyzw);
    
   

    
    
    psOut.MetallcEmissionRoughness.rgb = tex2D(merSampler, input.TexCoords).xyz;
    psOut.MetallcEmissionRoughness.a = 1;
    
    return psOut;

}

technique GBufferDepthPeeling
{
	pass P0
	{
        VertexShader = compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
};
/*matrix World;
matrix View;
matrix Projection;

 

float FarClip;

struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float3 Normal : TEXCOORD0;
    float4 vPositionVS : TEXCOORD1;
};

VS_OUTPUT DepthVertexShaderFunction(VS_INPUT IN)
{
    VS_OUTPUT Output;
    float4x4 WorldView;
    float4x4 ITWorldView;
    float4x4 WorldViewProjection;
    WorldViewProjection = World * View * Projection;
    WorldView = World * View;
    ITWorldView = World * View;
    Output.Position = mul(IN.Position, mul(mul(World, View),);
    Output.vPositionVS = mul(IN.Position, WorldView);
    Output.Normal = mul(IN.Normal, ITWorldView);

    return Output;
}

float4 DepthPixelShaderFunction(VS_OUTPUT IN) : COLOR
{
    float depth = IN.vPositionVS.z / 50;
    IN.Normal = normalize(IN.Normal);
    return float4(IN.Normal.x, IN.Normal.y, IN.Normal.z, depth);
}

technique Depth
{
    pass Pass1
    {

        VertexShader = compile vs_3_0 DepthVertexShaderFunction();
        PixelShader = compile ps_3_0 DepthPixelShaderFunction();
    }
}*/