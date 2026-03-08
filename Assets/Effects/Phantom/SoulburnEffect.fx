sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 center = float2(0.5, 0.5);
    
    float swayPhase = (uIntensity - 0.25) * 1.333;
    if (swayPhase < 0.0) swayPhase = 0.0;
    if (swayPhase > 1.0) swayPhase = 1.0;
    
    float drainPhase = (uIntensity - 0.5) * 2.0;
    if (drainPhase < 0.0) drainPhase = 0.0;
    if (drainPhase > 1.0) drainPhase = 1.0;
    
    float collapsePhase = (uIntensity - 0.8) * 5.0;
    if (collapsePhase < 0.0) collapsePhase = 0.0;
    if (collapsePhase > 1.0) collapsePhase = 1.0;

    float2 warpedCoords = coords;
    
    warpedCoords.x += sin(uTime * 2.0) * 0.012 * swayPhase;
    warpedCoords.y += cos(uTime * 1.5) * 0.012 * swayPhase;
    
    warpedCoords.x += sin(coords.y * 45.0 + uTime * 8.0) * 0.004 * swayPhase;
    
    warpedCoords.x += sin(warpedCoords.y * 20.0 + uTime * 10.0) * 0.03 * collapsePhase;
    
    warpedCoords += (warpedCoords - center) * sin(uTime * 10.0) * 0.06 * collapsePhase;
    
    warpedCoords.x += (frac(uTime * 23.0) - 0.5) * 0.015 * collapsePhase;
    warpedCoords.y += (frac(uTime * 31.0) - 0.5) * 0.015 * collapsePhase;
    
    float dist = distance(warpedCoords, center);
    
    float driftSpeed1 = uTime * 2.0;
    float driftSpeed2 = uTime * 1.5; 

    float scale1 = 1.0 - (0.04 * uIntensity * sin(uTime * 1.2)); 
    float2 offset1 = float2(sin(driftSpeed1), cos(driftSpeed1)) * 0.02 * uIntensity; 
    float2 coords1 = center + (warpedCoords - center) * scale1 + offset1;

    float scale2 = 1.0 + (0.04 * uIntensity * cos(uTime * 1.1));
    float2 offset2 = float2(cos(driftSpeed2), -sin(driftSpeed2)) * 0.025 * uIntensity;
    float2 coords2 = center + (warpedCoords - center) * scale2 + offset2;

    float2 baseOffset = float2(sin(uTime * 5.0), cos(uTime * 4.0)) * 0.002 * uIntensity;
    float4 baseColor = tex2D(uImage0, warpedCoords + baseOffset);
    float4 ghost1Color = tex2D(uImage0, coords1);
    float4 ghost2Color = tex2D(uImage0, coords2);
    
    float luminance = (baseColor.r * 0.299) + (baseColor.g * 0.587) + (baseColor.b * 0.114);
    float3 deadColor = float3(luminance * 0.4, luminance * 0.4, luminance * 0.8);
    float drainPulse = drainPhase * (0.9 + 0.1 * sin(uTime * 5.0));
    
    baseColor.rgb = baseColor.rgb + ((deadColor - baseColor.rgb) * drainPulse);
    
    ghost1Color.rgb *= float3(0.4, 1.0, 1.0); 
    ghost2Color.rgb *= float3(0.8, 0.3, 1.0); 

    float ghostAlpha = 0.6 * uIntensity;
    float4 finalColor = baseColor;
    finalColor.rgb = (baseColor.rgb + (ghost1Color.rgb * ghostAlpha) + (ghost2Color.rgb * ghostAlpha)) / (1.0 + ghostAlpha * 2.0);
    finalColor.a = baseColor.a;
    
    float vignetteRadius = 1.0 - (uIntensity * 0.35); 
    
    float vEdge0 = vignetteRadius - 0.4;
    float vEdge1 = vignetteRadius;
    float vPhase = (dist - vEdge0) / (vEdge1 - vEdge0);
    if (vPhase < 0.0) vPhase = 0.0;
    if (vPhase > 1.0) vPhase = 1.0;
    float vignetteAmount = vPhase * vPhase * (3.0 - 2.0 * vPhase);
    
    finalColor.rgb *= (1.0 - vignetteAmount);

    return finalColor;
}

technique Technique1
{
    pass SoulBurnEffectPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction(); 
    }
}