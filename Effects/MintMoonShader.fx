sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

float4 MintMoonPass(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float3 mintTint = float3(0.3, 1.0, 0.5);
    float baseTintStrength = 0.3;
    float tintStrength = baseTintStrength * uOpacity;
    color.rgb = lerp(color.rgb, color.rgb * mintTint, tintStrength);
    return lerp(tex2D(uImage0, coords), color, uOpacity);
}

technique Technique1
{
    pass MintMoonPass
    {
        PixelShader = compile ps_2_0 MintMoonPass();
    }
}