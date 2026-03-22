sampler uImage0 : register(s0);
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float uOpacity;    
float uIntensity;  
float uProgress;   

float4 CollapsingWavePass(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelCoords = coords * uScreenResolution;
    float2 center = uTargetPosition - uScreenPosition;
    float dist = distance(pixelCoords, center);
    
    float currentRadius = uOpacity * uProgress;
    
    float waveThickness = 15.0 + (uIntensity * 1000.0);
    
    float diff = abs(dist - currentRadius);
    float2 newCoords = coords;
    
    if (diff < waveThickness)
    {
        float waveFactor = 1.0 - (diff / waveThickness);
        float2 dir = normalize(pixelCoords - center);
        float distortion = sin(waveFactor * 3.14159) * uIntensity;
                float fadeOut = smoothstep(0.0, 0.25, uProgress);
                distortion *= fadeOut;
        
        newCoords -= dir * distortion;
    }
    
    return tex2D(uImage0, newCoords);
}

technique Technique1
{
    pass CollapsingWavePass
    {
        PixelShader = compile ps_2_0 CollapsingWavePass();
    }
}