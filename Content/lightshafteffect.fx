float gNumSamples = 64;

 
 

Texture2D maskTex;

sampler2D gTextureMask=sampler_state
{
    Texture = <maskTex>;
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

float2 gScreenLightPos;

float gDensity=1.0;

float gDecay;

float gWeight;

float gExposure;

 

sampler2D noiseTex=sampler_state
{
    Texture = <NoiseTex>;
    AddressU = Wrap;
    AddressV = Wrap;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
struct PS_IN
{
    float4 position : SV_POSITION;
   
    float2 texCoord : TEXCOORD0;
};

 
 
float4 PixelShaderFunction(PS_IN input) : SV_TARGET0
{
    
  
    float2 TexCoord = input.texCoord;
	// Calculate vector from pixel to light source in screen space.
    float noiseVal = tex2D(noiseTex, TexCoord*5).x * 0.1 + 0.95;
    float2 DeltaTexCoord = (TexCoord.xy - gScreenLightPos.xy) * noiseVal;
//    float Len = length(DeltaTexCoord);
	// Divide by number of samples and scale by control factor.
    DeltaTexCoord *=1.0 / 24 ;
	// Store initial sample.
    float3 Color = tex2D(gTextureMask, TexCoord).xyz;
 
	// Set up illumination decay factor.
    float IlluminationDecay = 1.0;
    float3 Sample;
    
	// Evaluate summation from Equation 3 ( see https://developer.nvidia.com/gpugems/GPUGems3/gpugems3_ch13.html) NUM_SAMPLES iterations.
     [unroll]
    for (int i = 0; i < 24 ; ++i)
    {
		// Step sample location along ray.
        TexCoord -= DeltaTexCoord;
        if (TexCoord.x < 0 || TexCoord.x > 1 || TexCoord.y < 0 || TexCoord.y > 1)
        {
            return float4(0,0, 0, 1);
        }
		// Retrieve sample at new location.
        Sample = tex2D(gTextureMask, TexCoord).xyz;
		// Apply sample attenuation scale/decay factors.
        Sample *= IlluminationDecay * gWeight*gDensity;
		// Accumulate combined color.
        Color += Sample;
		// Update exponential decay factor.
        IlluminationDecay *= gDecay;
    }
    
	// Output final color with a further scale control factor.
        return float4(Color.xyz * gExposure, 1);
}
 

technique LightShaft
{

    pass p0
    {

       

        PixelShader = compile ps_4_0 PixelShaderFunction();

    }

}

 