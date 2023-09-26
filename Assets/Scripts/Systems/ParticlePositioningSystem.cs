using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
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
		ParticleTimingData particleTimingData = SystemAPI.GetSingleton<ParticleTimingData>();
		particleTimingData.m_timePassed += SystemAPI.Time.DeltaTime;
		if(particleTimingData.m_timePassed > particleTimingData.m_timePerIndex)
		{
			particleTimingData.m_timePassed -= particleTimingData.m_timePerIndex;
			particleTimingData.m_timeIndex += 1;
			if(particleTimingData.m_timeIndex >= particleTimingData.m_numberIndices)
				particleTimingData.m_timeIndex = 0;
		}

		new PositionParticleJob
		{
			m_timeIndex = particleTimingData.m_timeIndex
		}.ScheduleParallel();
	}
}

[BurstCompile]
public partial struct PositionParticleJob : IJobEntity
{
	public int m_timeIndex;

	[BurstCompile]
	private void Execute(ParticlePositionAspect a_particle)
	{
		a_particle.SetTimeIndex(m_timeIndex);
	}
}
