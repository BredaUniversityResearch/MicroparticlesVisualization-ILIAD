using CesiumForUnity;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;

[BurstCompile]
public partial struct ParticlePositioningSystem : ISystem
{
    private EntityQuery query;

    [BurstCompile]
    public void OnCreate(ref SystemState a_state)
    {
        a_state.RequireForUpdate<ParticleTimingData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState a_state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState a_state)
    {
        Entity particleTimingEnt = SystemAPI.GetSingletonEntity<ParticleTimingData>();
        Entity abstractMapDataEnt = SystemAPI.GetSingletonEntity<AbstractMapData>();
        ParticleTimingAspect particleTimingAspect = SystemAPI.GetAspect<ParticleTimingAspect>(particleTimingEnt);
        AbstractMapData abstractMapData = SystemAPI.GetComponent<AbstractMapData>(abstractMapDataEnt);
        float time = particleTimingAspect.IndexAtTime(abstractMapData.timelineValue * particleTimingAspect.TotalTime);

        new PositionParticleJob
        {
            Time = time,
            ECEFtoLocal = abstractMapData.ECEFMatrix,
            CameraHeight = abstractMapData.cameraHeight,
            ParticleSize = abstractMapData.particleSize,
            ParticleMinSize = abstractMapData.particleMinSize
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct PositionParticleJob : IJobEntity
{
    public float Time;
    public double4x4 ECEFtoLocal;
    public float CameraHeight;
    public float ParticleSize;
    public float ParticleMinSize;

    [BurstCompile]
    private void Execute(ParticleUpdateAspect a_particle)
    {
        // Get the particle position in longitude/latitude/depth values.
        var pos = a_particle[Time];

        float rg = pow(1f - abs(pos.z) / 100f, 2);
        float b = 1f - pow(abs(pos.z) / 100f, 2);

        a_particle.Colour = float4(rg, rg, b, 1f);
        pos = GeoToLocalPosition(pos);
        a_particle.Position = pos;

        // Scale the particle based on camera elevation.
        a_particle.Scale = max(CameraHeight * ParticleSize, ParticleMinSize);
    }

    [BurstCompile]
    public float3 GeoToLocalPosition(float3 longitudeLatitudeDepth)
    {
        var ecef = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(longitudeLatitudeDepth);
        var pos = mul(ECEFtoLocal, double4(ecef, 1.0)).xyz;
        return float3(pos);
    }
}
