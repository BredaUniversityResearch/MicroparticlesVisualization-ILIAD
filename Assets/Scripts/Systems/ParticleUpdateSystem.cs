using CesiumForUnity;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;

[BurstCompile]
public partial struct ParticlePositioningSystem : ISystem
{
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
        if (SystemAPI.HasSingleton<ParticleTimingData>() && SystemAPI.HasSingleton<AbstractMapData>())
        {
            Entity particleTimingEnt = SystemAPI.GetSingletonEntity<ParticleTimingData>();
            Entity abstractMapDataEnt = SystemAPI.GetSingletonEntity<AbstractMapData>();
            ParticleTimingAspect particleTimingAspect = SystemAPI.GetAspect<ParticleTimingAspect>(particleTimingEnt);
            AbstractMapData abstractMapData = SystemAPI.GetComponent<AbstractMapData>(abstractMapDataEnt);
            int timeIndex = particleTimingAspect.IndexAtTime(abstractMapData.timelineValue * particleTimingAspect.TotalTime);

            new PositionParticleJob
            {
                TimeIndex = timeIndex,
                ECEFtoLocal = abstractMapData.ECEFMatrix,
                CameraPosition = abstractMapData.cameraPosition,
                CameraHeight = abstractMapData.cameraHeight,
                ParticleSize = abstractMapData.particleSize
            }.ScheduleParallel();
        }
    }
}

[BurstCompile]
public partial struct PositionParticleJob : IJobEntity
{
    public int TimeIndex;
    public double4x4 ECEFtoLocal;
    public float3 CameraPosition;
    public float CameraHeight;
    public float ParticleSize;

    [BurstCompile]
    private void Execute(ParticleUpdateAspect a_particle)
    {
        // Get the particle position in longitude/latitude/depth values.
        var pos = a_particle[TimeIndex];

        float rg = pow(1f - abs(pos.z) / 100f, 2);
        float b = 1f - pow(abs(pos.z) / 100f, 2);

        a_particle.Colour = float4(rg, rg, b, 1f);
        pos = GeoToLocalPosition(pos);
        a_particle.Position = pos;

        // Scale the particle based on distance to the camera.
        var d = distance(CameraPosition, pos);
        a_particle.Scale = d * ParticleSize;
        //a_particle.Scale = CameraHeight * ParticleSize;
    }

    [BurstCompile]
    public float3 GeoToLocalPosition(float3 longitudeLatitudeDepth)
    {
        var ecef = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(longitudeLatitudeDepth);
        var pos = mul(ECEFtoLocal, double4(ecef, 1.0)).xyz;
        return float3(pos);
    }
}
