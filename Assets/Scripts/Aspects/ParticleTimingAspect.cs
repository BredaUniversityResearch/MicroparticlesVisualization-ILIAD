using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ParticleTimingAspect : IAspect
{
	public readonly Entity Entity;

	private readonly RefRW<ParticleTimingData> m_particleTiming;

	public int PassTime(float a_deltaTime)
	{
		m_particleTiming.ValueRW.m_timePassed += a_deltaTime;
		if (m_particleTiming.ValueRW.m_timePassed > m_particleTiming.ValueRW.m_timePerIndex)
		{
			m_particleTiming.ValueRW.m_timePassed -= m_particleTiming.ValueRW.m_timePerIndex;
			m_particleTiming.ValueRW.m_timeIndex += 1;
			if (m_particleTiming.ValueRW.m_timeIndex >= m_particleTiming.ValueRW.m_numberIndices)
				m_particleTiming.ValueRW.m_timeIndex = 0;
		}
		return m_particleTiming.ValueRW.m_timeIndex;
	}
}
