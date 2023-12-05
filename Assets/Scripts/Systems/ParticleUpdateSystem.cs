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
    { }

    [BurstCompile]
    public void OnUpdate(ref SystemState a_state)
    {
        if (SystemAPI.HasSingleton<AbstractMapData>())
        {
            Entity particleTimingEnt = SystemAPI.GetSingletonEntity<ParticleTimingData>();
            ParticleTimingAspect particleTimingAspect = SystemAPI.GetAspect<ParticleTimingAspect>(particleTimingEnt);
            int timeIndex = particleTimingAspect.PassTime(SystemAPI.Time.DeltaTime);
            
            Entity abstractMapDataEnt = SystemAPI.GetSingletonEntity<AbstractMapData>();
            AbstractMapData abstractMapData = SystemAPI.GetComponent<AbstractMapData>(abstractMapDataEnt);

            new PositionParticleJob
            {
                TimeIndex = timeIndex,
                ECEFtoLocal = abstractMapData.ECEFMatrix
            }.ScheduleParallel();
        }
    }
}

[BurstCompile]
public partial struct PositionParticleJob : IJobEntity
{
    public int TimeIndex;
    public double4x4 ECEFtoLocal;

    [BurstCompile]
    private void Execute(ParticleUpdateAspect a_particle)
    {
        // Get the particle position in longitude/latitude/depth values.
        var pos = a_particle[TimeIndex];

        float rg = pow(1f - abs(pos.z) / 100f, 2);
        float b = 1f - pow(abs(pos.z) / 100f, 2);

        a_particle.Colour = float4(rg, rg, b, 1f);
        a_particle.Position = GeoToLocalPosition(pos);
    }

    [BurstCompile]
    public float3 GeoToLocalPosition(float3 longitudeLatitudeDepth)
    {
        var ecef = CesiumWgs84Ellipsoid.LongitudeLatitudeHeightToEarthCenteredEarthFixed(longitudeLatitudeDepth);
        var pos = mul(ECEFtoLocal, double4(ecef, 1.0)).xyz;
        return float3(pos);
    }
}
