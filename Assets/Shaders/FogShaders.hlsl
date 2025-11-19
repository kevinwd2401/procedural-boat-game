
static const float STEP_SIZE = 0.1;
static const float3 LIGHT_DIR = float3(0.8, 0.5, 0.2);
static const float3 LIGHT_COLOR = float3(0.16, 0.17, 0.25);
static const float NOISE_SCALE = 0.003;
static const float NOISE_STRENGTH = 0.8;


float2 rayBoxDist(float3 BoundsMin, float3 BoundsMax, float3 RayOrigin, float3 RayDirection)
{
    float3 t0 = (BoundsMin - RayOrigin) / RayDirection;
    float3 t1 = (BoundsMax - RayOrigin) / RayDirection;

    float3 tmin = min(t0, t1);
    float3 tmax = max(t0, t1);

    float distA = max(max(tmin.x, tmin.y), tmin.z);
    float distB = min(min(tmax.x, tmax.y), tmax.z);

    float distToBox = max(0, distA);
    float distInsideBox = max(0, distB - distToBox);

    return float2(distToBox, distInsideBox);
}

float sampleDensity(float3 Position)
{
    return 0.04;
}

float sampleDensityDissipation(float3 Position, float3 textWorldCenter, float texWorldSize, UnityTexture2D DissipationTex,
    UnitySamplerState DissipationSampler, UnityTexture3D NoiseTex, UnitySamplerState NoiseSampler, float time, float fogMultiplier)
{
        
    float3 noiseUV = Position * NOISE_SCALE;
    noiseUV.y += time;

    float noiseValue = SAMPLE_TEXTURE3D(NoiseTex, NoiseSampler, noiseUV).r;
    
    float noiseFactor = lerp(1.0 - NOISE_STRENGTH, 1.0 + NOISE_STRENGTH, noiseValue);
    float baseDensity = saturate(0.6 * fogMultiplier * noiseFactor);

    float2 localPos = (textWorldCenter.xz - Position.xz) / texWorldSize + 0.5;
    
    float dissipation = max(0, SAMPLE_TEXTURE2D(DissipationTex, DissipationSampler, localPos).r);

    if (any(localPos < 0.0) || any(localPos > 1.0))
        return baseDensity;
    
    return lerp(baseDensity, baseDensity * 0.05, clamp(dissipation, 0, 1));
}

float lightmarch(float3 Position, float3 BoundsMin, float3 BoundsMax)
{
    float3 dirToLight = normalize(LIGHT_DIR);
    float distInsideBox = rayBoxDist(BoundsMin, BoundsMax, Position, dirToLight).y;

    float totalDensity = 0;
    int steps = (int) (distInsideBox / STEP_SIZE);

    for (int i = 0; i < steps; i++)
    {
        Position += dirToLight * STEP_SIZE;
        totalDensity += max(0, sampleDensity(Position) * STEP_SIZE);
    }

    float transmittance = exp(-totalDensity);
    return transmittance;
}

void RayMarcher_float(
    float4 SceneColor,
    float3 RayOrigin,
    float3 RayDirection,
    float3 BoundsMin,
    float3 BoundsMax,
    float Depth,
    float3 RayDirectionView,
    float texWorldSize,
    UnityTexture2D DissipationTex,
    UnitySamplerState DissipationSampler,
    float3 texCenter,
    UnityTexture3D NoiseTex,
    UnitySamplerState NoiseSampler,
    float time,
    float fogMultiplier,
    out float4 outValue
)
{
    float3 dirNorm = normalize(RayDirection);

    float2 RayBoxInfo = rayBoxDist(BoundsMin, BoundsMax, RayOrigin, dirNorm);

    float4 sceneCol = SceneColor;

    float distToPoint = Depth / abs(RayDirectionView.z);

    float distTravelled = 0;
    float distLimit = min(distToPoint - RayBoxInfo.x, RayBoxInfo.y);

    float transmittance = 1;
    float lightEnergy = 0;
    
    [loop]
    for (int i = 0; i < 64; i++)
    {
        if (distTravelled >= distLimit)
            break;

        float3 marchPos = RayOrigin + dirNorm * (RayBoxInfo.x + distTravelled);

        float density = sampleDensityDissipation(
            marchPos,
            texCenter,
            texWorldSize,
            DissipationTex,
            DissipationSampler,
            NoiseTex,
            NoiseSampler,
            time,
            fogMultiplier
        );

        if (density > 0.0)
        {
            //float lightTransmittance = lightmarch(marchPos, BoundsMin, BoundsMax);

            lightEnergy += density * STEP_SIZE * transmittance; // * lightTransmittance;
            transmittance *= exp(-density * STEP_SIZE);

            if (transmittance < 0.01)
                break;
        }

        distTravelled += STEP_SIZE;
    }

    float3 finalCol = sceneCol.rgb * transmittance + lightEnergy * LIGHT_COLOR;
    outValue = float4(finalCol, 0);
}
