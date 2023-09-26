using Unity.Collections;
using Unity.Entities;

public struct ParticleProperties : IComponentData
{
	public NativeArray<float> m_lats;
	public NativeArray<float> m_lons;
	public NativeArray<float> m_depths;
}

