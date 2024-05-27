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
			//TODO: Implement type info within abstractMapData.
			//ParticleType = abstractMapData.particleType,

			ParticleColour = particleVisualizationSettingsData.m_colourIndex,
            ParticleDarkness = particleVisualizationSettingsData.m_darknessIndex

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

    [BurstCompile]
    private void Execute(ParticleUpdateAspect a_particle)
    {
        switch(ParticleColour)
        {
            case 0:
                SetTypeColour(a_particle);
                break;
            case 1:
                SetSizeColour(a_particle);
                break;
            case 2:
                SetDepthColour(a_particle);
                break;
        }

        switch(ParticleDarkness)
        {
            case 0:
                SetTypeDarkness(a_particle);
                break;
            case 1:
                SetSizeDarkness(a_particle);
                break;
            case 2:
                SetDepthDarkness(a_particle);
                break;
        }

        SetPositionAndScale(a_particle);
        a_particle.ApplyFilter(Time, SizeDepthFilter, TypeFilter);

	}

    [BurstCompile]
    private void SetDepthColour(ParticleUpdateAspect a_particle)
    {
        // Get the particle position in longitude/latitude/depth values.
        var pos = a_particle[Time];

        float b = 1f - pow(abs(pos.z) / 100f, 2);

        a_particle.ColourGradient = b;
    }

    [BurstCompile]
    private void SetDepthDarkness(ParticleUpdateAspect a_particle)
    {
        a_particle.ColourGradient = 1f;

        var pos = a_particle[Time];

        float b = 1f - pow(abs(pos.z) / 100f, 2);

        b = 0.1f + b * 0.9f;

        a_particle.Darkness = b;
    }
    
    [BurstCompile]
    private void SetTypeColour(ParticleUpdateAspect a_particle)
    {
        //TODO: Set the colour based on the particle type.
        //This will use the rainbow gradient.
    }

    [BurstCompile]
    private void SetTypeDarkness(ParticleUpdateAspect a_particle)
    {

    }
    
    [BurstCompile]
    private void SetSizeColour(ParticleUpdateAspect a_particle)
    {

    }

    [BurstCompile]
    private void SetSizeDarkness(ParticleUpdateAspect a_particle)
    {

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
