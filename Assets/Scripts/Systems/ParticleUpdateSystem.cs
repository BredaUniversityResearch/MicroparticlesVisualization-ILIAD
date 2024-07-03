using CesiumForUnity;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
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
        Entity particleTimingEnt = SystemAPI.GetSingletonEntity<ParticleTimingData>();
        Entity abstractMapDataEnt = SystemAPI.GetSingletonEntity<AbstractMapData>();

		ParticleTimingAspect particleTimingAspect = SystemAPI.GetAspect<ParticleTimingAspect>(particleTimingEnt);
        AbstractMapData abstractMapData = SystemAPI.GetComponent<AbstractMapData>(abstractMapDataEnt);
        ParticleVisualizationSettingsData particleVisualizationSettingsData = SystemAPI.GetComponent<ParticleVisualizationSettingsData>(SystemAPI.GetSingletonEntity<ParticleVisualizationSettingsData>());

		float time = particleTimingAspect.IndexAtTime(abstractMapData.timelineValue * particleTimingAspect.TotalTime);

        new PositionParticleJob
        {
            Time = time,
            ECEFtoLocal = abstractMapData.ECEFMatrix,
            CameraHeight = abstractMapData.cameraHeight,
            ParticleSize = abstractMapData.particleSize,
            ParticleMinSize = abstractMapData.particleMinSize,
            SizeDepthFilter = particleVisualizationSettingsData.m_sizeDepthFilter,
            TypeFilter = particleVisualizationSettingsData.m_typeFilter,
			ParticleColour = particleVisualizationSettingsData.m_colourIndex,
            ParticleDarkness = particleVisualizationSettingsData.m_darknessIndex,
            NumberTypes = particleVisualizationSettingsData.m_numberTypes
        }.ScheduleParallel();
    }
}

[BurstCompile, WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct PositionParticleJob : IJobEntity
{
    public float Time;
    public double4x4 ECEFtoLocal;
    public float CameraHeight;
    public float ParticleSize;
    public float ParticleMinSize;
    public int ParticleColour;
    public int ParticleDarkness;
    public float4 SizeDepthFilter;
    public int TypeFilter;
    public int NumberTypes;

    [BurstCompile]
    private void Execute(ParticleUpdateAspect a_particle)
    {
        switch(ParticleColour)
        {
            case 0:
                a_particle.ColourGradient = GetTypeT(a_particle);
                break;
            case 1:
                a_particle.ColourGradient = GetSizeT(a_particle);
                break;
            case 2:
                a_particle.ColourGradient = GetDepthT(a_particle);
                break;
        }

        switch(ParticleDarkness)
        {
            case 0:
                a_particle.Darkness = GetTypeT(a_particle);
                break;
            case 1:
                a_particle.Darkness = GetSizeT(a_particle);
                break;
            case 2:
                a_particle.Darkness = GetDepthT(a_particle);
                break;
        }

        SetPositionAndScale(a_particle);
        a_particle.ApplyFilter(Time, SizeDepthFilter, TypeFilter);

	}

    [BurstCompile]
    private float GetDepthT(ParticleUpdateAspect a_particle)
	{
        return (a_particle[Time].z - SizeDepthFilter[2]) / (SizeDepthFilter[3] - SizeDepthFilter[2]);
    }

    [BurstCompile]
    private float GetSizeT(ParticleUpdateAspect a_particle)
    {
        return (a_particle.ParticleSize - SizeDepthFilter[2]) / (SizeDepthFilter[3] - SizeDepthFilter[2]);
    }

    [BurstCompile]
    private float GetTypeT(ParticleUpdateAspect a_particle)
    {
        return (float)a_particle.ParticleType / (float)(NumberTypes-1);
    }

	//[BurstCompile]
	private void SetPositionAndScale(ParticleUpdateAspect a_particle)
    {
        // Get the particle position in longitude/latitude/depth values.
        var pos = a_particle[Time];

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
