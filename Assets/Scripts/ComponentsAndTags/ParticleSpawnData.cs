using Unity.Collections;
using Unity.Entities;

public struct ParticleSpawnData : IComponentData
{
	public NativeArray<float> m_lats;
	public NativeArray<float> m_lons;
	public NativeArray<float> m_depths;
	public int m_entriesPerParticle;
}
