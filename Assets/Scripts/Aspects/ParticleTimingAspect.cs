using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ParticleTimingAspect : IAspect
{
	public readonly Entity Entity;

	private readonly RefRW<ParticleTimingData> m_particleTiming;

	/// <summary>
	/// Get the total amount of time of the simulation.
	/// </summary>
    public float TotalTime => m_particleTiming.ValueRO.m_numberIndices * m_particleTiming.ValueRO.m_timePerIndex;

	/// <summary>
	/// Get the index at a specific moment in time.
	/// </summary>
	/// <param name="time">The the time interval to query.</param>
	/// <returns></returns>
    public int IndexAtTime(float time)
    {
        time = time % TotalTime; // Wrap around.
        int index = (int)(time / m_particleTiming.ValueRO.m_timePerIndex);
		return index;
    }
}
