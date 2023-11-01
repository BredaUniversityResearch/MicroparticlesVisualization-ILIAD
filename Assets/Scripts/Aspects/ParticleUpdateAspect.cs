using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ParticleUpdateAspect : IAspect
{
	public readonly Entity Entity;
	public const float m_xyScale = 5f;
	public const float m_zScale = 0.1f;

	private readonly RefRW<LocalTransform> m_transform;
	private readonly RefRO<ParticleProperties> m_particleProperties;
	private readonly RefRW<ParticleColourComponent> m_colour;

	public void SetTimeIndex(int a_index)
	{
		float depth = m_particleProperties.ValueRO.Value.Value.m_depths[a_index];
		float rg = math.pow(1f - math.abs(depth) / 100f, 2);
		float b = 1f - math.pow( math.abs(depth) / 100f, 2);
		m_colour.ValueRW.Value = new float4( rg, rg, b, 1f);
		m_transform.ValueRW.Position = new float3( (m_particleProperties.ValueRO.Value.Value.m_lons[a_index] - 10.476f) * m_xyScale, (m_particleProperties.ValueRO.Value.Value.m_lats[a_index] - 63.583f) * m_xyScale, depth * m_zScale);
	}
}
