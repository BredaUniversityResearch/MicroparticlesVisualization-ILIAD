using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

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
        Entity particleTimingEnt = SystemAPI.GetSingletonEntity<ParticleTimingData>();
        Entity abstractMapData = SystemAPI.GetSingletonEntity<AbstractMapData>();

        ParticleTimingAspect particleTimingAspect = SystemAPI.GetAspect<ParticleTimingAspect>(particleTimingEnt);
        int timeIndex = particleTimingAspect.PassTime(SystemAPI.Time.DeltaTime);

        AbstractMapAspect abstractMapAspect = SystemAPI.GetAspect<AbstractMapAspect>(abstractMapData);

        new PositionParticleJob
        {
            m_timeIndex = timeIndex,
            m_MapAspect = abstractMapAspect
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct PositionParticleJob : IJobEntity
{
    public int m_timeIndex;
    
    // Is it safe to use an aspect in a Job?
    [NativeDisableUnsafePtrRestriction] 
    public AbstractMapAspect m_MapAspect;

    [BurstCompile]
    private void Execute(ParticlePositionAspect a_particle)
    {
        //		a_particle.SetTimeIndex(m_timeIndex);
        a_particle.Position = m_MapAspect.GeoToWorldPosition(a_particle[m_timeIndex]);
    }
}
