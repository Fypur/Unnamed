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
    
    /*if (input.TextureCoordinates.x < 0.03f)
    {
        color.rgb *= input.TextureCoordinates.x * 33;
        
    }*/
    
    float2 uv = input.TextureCoordinates.xy;
   
    uv *= 1 - uv.yx; //vec2(1.0)- uv.yx; -> 1.-u.yx; Thanks FabriceNeyret !
    
    float vig = uv.x * uv.y * strength; // multiply with sth for intensity
    
    vig = pow(abs(vig), extent); // change pow for modifying the extend of the  vignette

    if(vig < 1.0f)
        color.rgb *= vig;
        
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