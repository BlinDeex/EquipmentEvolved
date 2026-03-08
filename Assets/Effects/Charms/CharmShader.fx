sampler uImage0 : register(s0);

float3 uColor;
float uOpacity;

float4 DrawCircle(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords - 0.5;
    float dist = length(uv) - 0.35;
    float alpha = 1.0 - smoothstep(0.0, 0.015, dist);
    return float4(uColor, 1.0) * alpha * uOpacity;
}

float4 DrawSquare(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords - 0.5;
    float dist = max(abs(uv.x), abs(uv.y)) - 0.35;
    float alpha = 1.0 - smoothstep(0.0, 0.015, dist);
    return float4(uColor, 1.0) * alpha * uOpacity;
}

float4 DrawTriangle(float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords - 0.5;
    uv.y += 0.1;
    float dist = max(abs(uv.x) * 0.866025 + uv.y * 0.5, -uv.y) - (0.35 * 0.5);
    float alpha = 1.0 - smoothstep(0.0, 0.015, dist);
    return float4(uColor, 1.0) * alpha * uOpacity;
}

technique Circle
{
    pass P0
    {
        PixelShader = compile ps_2_0 DrawCircle();
    }
}

technique Square
{
    pass P0
    {
        PixelShader = compile ps_2_0 DrawSquare();
    }
}

technique Triangle
{
    pass P0
    {
        PixelShader = compile ps_2_0 DrawTriangle();
    }
}