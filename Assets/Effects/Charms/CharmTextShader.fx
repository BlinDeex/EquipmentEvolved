sampler uImage0 : register(s0);
float uTime;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 vertexColor : COLOR0) : COLOR0
{
    float r = vertexColor.r;
    float g = vertexColor.g;
    float b = vertexColor.b;

    float maxVal = max(r, max(g, b));
    
    if (maxVal <= 0.01)
    {
        float4 pColor = tex2D(uImage0, coords);
        return float4(vertexColor.rgb, 1.0) * pColor * vertexColor.a;
    }

    float nr = r / maxVal;
    float ng = g / maxVal;
    float nb = b / maxVal;

    // --- 6. EXALTED (Pure Green Secret Key: nr=0, ng=1, nb=0) ---
        if (ng > 0.9 && nr < 0.2 && nb < 0.2)
        {
            float sliceY = floor(coords.y * 120.0); 
            float offset = sin(sliceY * 15.0 + uTime * 25.0) * 0.008;
            
            float glitchSpike = step(0.96, sin(uTime * 12.0 + sliceY * 3.0));
            offset += glitchSpike * 0.04 * sin(uTime * 60.0);
            
            float2 shatteredCoords = coords + float2(offset, 0.0);
    
            // Sample the fractured texture
            float4 pColor = tex2D(uImage0, shatteredCoords);
            if (pColor.a <= 0.0) return float4(0, 0, 0, 0);
    
            // 2. UNSTABLE QUANTUM ENERGY (High-contrast Electric Teal, White, & Black)
            float energy = sin(shatteredCoords.x * 60.0 + uTime * 40.0) 
                         * cos(shatteredCoords.y * 50.0 - uTime * 35.0);
                         
            // pow(abs()) makes the energy bands sharp like lightning instead of soft clouds
            energy = pow(abs(energy), 0.4); 
    
            float3 voidColor = float3(0.02, 0.0, 0.05); // Pitch black void
            float3 cyanCore = float3(0.0, 0.9, 1.0);    // Searing electric teal
            float3 whiteHot = float3(2.0, 2.0, 2.0);    // Blinding, overblown white
    
            // Create the core anomaly colors
            float3 finalAnomaly = lerp(voidColor, cyanCore, smoothstep(0.3, 0.8, energy));
            
            finalAnomaly += smoothstep(0.85, 1.0, energy) * whiteHot;
            
            float staticNoise = frac(sin(dot(shatteredCoords * uTime, float2(12.9898, 78.233))) * 43758.5453);
            
            // The static only appears heavily on the parts of the text that are actively glitching/tearing
            finalAnomaly += staticNoise * 0.6 * glitchSpike * cyanCore; 
            
            float crashFlash = step(0.95, sin(uTime * 15.0));
            finalAnomaly = lerp(finalAnomaly, float3(1.0, 1.0, 1.0) - finalAnomaly, crashFlash);
    
            return float4(finalAnomaly, 1.0) * pColor * vertexColor.a;
        }
        
    float4 pixelColor = tex2D(uImage0, coords);
    if (pixelColor.a <= 0.0) return float4(0, 0, 0, 0);

    float3 finalColor = vertexColor.rgb;

    // 1. COMMON
    if (nr > 0.9 && ng > 0.9 && nb > 0.9)
    {
        float pulse = sin(uTime * 2.0) * 0.15 + 0.85; 
        finalColor *= pulse;
    }
    // 2. RARE 
    else if (nb > 0.9 && nr < 0.6 && ng < 0.6)
    {
        float wave1 = sin(coords.x * 60.0 + uTime * 4.0);
        float wave2 = cos(coords.y * 60.0 - uTime * 3.0);
        float continuousNoise = (wave1 + wave2) * 0.5;

        float pulse = sin(uTime * 3.0) * 0.1 + 0.9;
        finalColor *= pulse;
        finalColor += max(0.0, continuousNoise) * 0.5 * float3(0.0, 1.0, 1.0) * maxVal; 
    }
    // 3. EPIC 
    else if (nr > 0.9 && nb > 0.9 && ng < 0.2)
    {
        float explosivePulse = pow(sin(uTime * 8.0) * 0.5 + 0.5, 4.0); 
        float scanline = sin(coords.x * 50.0 - coords.y * 50.0 + uTime * 15.0);
        float neonStrike = smoothstep(0.85, 1.0, scanline); 

        float3 deepPurple = float3(0.4, 0.0, 0.6) * maxVal;
        float3 basePink = float3(1.0, 0.2, 1.0) * maxVal;
        
        finalColor = lerp(deepPurple, basePink, sin(uTime * 3.0) * 0.5 + 0.5);
        finalColor += neonStrike * float3(1.0, 0.5, 1.0) * maxVal; 
        finalColor += explosivePulse * float3(0.8, 0.0, 0.8) * maxVal; 
    }
    // 4. LEGENDARY 
    else if (nr > 0.9 && ng > 0.9 && nb < 0.2)
    {
        float flow = sin(coords.x * 20.0 + coords.y * 30.0 + uTime * 4.0) 
                   + sin(coords.x * -15.0 + coords.y * 25.0 + uTime * 3.0);
        flow = flow * 0.25 + 0.5; 
        
        float3 darkGold = float3(0.6, 0.35, 0.0) * maxVal;
        float3 pureGold = float3(1.0, 0.85, 0.1) * maxVal;
        finalColor = lerp(darkGold, pureGold, flow);

        float glint = smoothstep(0.95, 1.0, sin(coords.x * 60.0 - uTime * 12.0));
        finalColor += glint * float3(1.0, 1.0, 0.8) * maxVal * 1.5; 
    }
    // 5. MYTHICAL 
    else if (nr > 0.9 && ng < 0.6 && nb < 0.6)
    {
        float n1 = sin(coords.x * 120.0 + uTime * 8.0);
        float n2 = cos(coords.y * 120.0 - uTime * 6.0);
        float surfaceTexture = abs(n1 * n2); 
        
        float3 darkRed = float3(0.3, 0.0, 0.0) * maxVal;
        float3 vividRed = float3(1.0, 0.1, 0.1) * maxVal;
        finalColor = lerp(darkRed, vividRed, surfaceTexture * 0.7 + 0.3);

        float sweepPos = coords.x * 60.0 + coords.y * 60.0 - uTime * 18.0;
        float coreGlint = smoothstep(0.92, 1.0, sin(sweepPos));
        float fringeGlint = smoothstep(0.7, 0.92, sin(sweepPos)) - coreGlint;

        finalColor += coreGlint * float3(1.0, 1.0, 1.0) * maxVal * 1.5;
        finalColor += fringeGlint * float3(1.0, 0.5, 0.0) * maxVal * 2.5 * surfaceTexture;

        float sparkle = pow(surfaceTexture, 8.0) * (sin(uTime * 15.0) * 0.5 + 0.5);
        finalColor += sparkle * float3(1.0, 0.8, 0.4) * maxVal * 1.5;
    }

    return float4(finalColor, 1.0) * pixelColor * vertexColor.a;
}

technique Technique1
{
    pass TextTestPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}