float3 MMLight(float4 ambientColor, float3 viewN, int lightCount)
{
    float3 lighting = float3(0, 0, 0);
    for (int i = 0; i < lightCount; i++) {
        lighting += unity_LightColor[i] * max(dot(unity_LightPosition[i].xyz, viewN), 0) * (1 - unity_LightPosition[i].w);
    }
    lighting += unity_AmbientSky.rgb * ambientColor;

    lighting = min(1, max(0, lighting));
    return lighting;
}

float3 MMLight3(float4 ambientColor, float3 viewN) {
    return MMLight(ambientColor, viewN, 3);
}

float3 MMLight8(float4 ambientColor, float3 viewN) {
    return MMLight(ambientColor, viewN, 8);
}