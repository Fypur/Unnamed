sampler inputTexture;
float barLocation;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 AllWhite(VertexShaderOutput inputV) : COLOR0
{
    float4 color = tex2D(inputTexture, inputV.TextureCoordinates) * inputV.Color;
    
    if (barLocation > 0 && abs(inputV.TextureCoordinates.y - barLocation) < 0.05f)
    {
        color.rgb = 1.0f;
    }
    
    return color;
}

technique tech
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile ps_3_0 AllWhite();
    }
};