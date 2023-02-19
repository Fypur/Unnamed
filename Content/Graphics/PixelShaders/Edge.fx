sampler inputTexture;
float strength;
float extent;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(inputTexture, input.TextureCoordinates) * input.Color;

    color.rgb = 255; 

    return color;
}

technique tech
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_3_0 MainPS();
    }
};