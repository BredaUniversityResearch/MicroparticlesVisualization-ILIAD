using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct ParticleUpdateSystem : ISystem
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
		Entity particleTimingEnt= SystemAPI.GetSingletonEntity<ParticleTimingData>();
		ParticleTimingAspect particleTimingAspect = SystemAPI.GetAspect<ParticleTimingAspect>(particleTimingEnt);
		int timeiIndex = particleTimingAspect.PassTime(SystemAPI.Time.DeltaTime);

		new PositionParticleJob
		{
			m_timeIndex = timeiIndex
		}.ScheduleParallel();
	}
}

[BurstCompile]
public partial struct PositionParticleJob : IJobEntity
{
	public int m_timeIndex;

	[BurstCompile]
	private void Execute(ParticleUpdateAspect a_particle)
	{
		a_particle.SetTimeIndex(m_timeIndex);
	}
}
