using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ParticlePositionAspect : IAspect
{
	public readonly Entity Entity;

	private readonly RefRW<LocalTransform> m_transform;
	private readonly RefRO<ParticleProperties> m_particleProperties;

	public void SetTimeIndex(int a_index)
	{
		m_transform.ValueRW.Position = new float3( m_particleProperties.ValueRO.Value.Value.m_lons[a_index] - 10.476f, m_particleProperties.ValueRO.Value.Value.m_lats[a_index] - 63.583f, m_particleProperties.ValueRO.Value.Value.m_depths[a_index]);
	}
}
