using Unity.Collections;
using Unity.Entities;

public struct ParticleTimingData : IComponentData
{
	public int m_timeIndex;
	public int m_numberIndices;
	public float m_timePassed;
	public float m_timePerIndex;
}