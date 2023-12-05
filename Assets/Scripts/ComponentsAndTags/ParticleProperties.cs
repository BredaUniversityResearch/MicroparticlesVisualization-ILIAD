using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ParticleProperties : IComponentData
{
    public BlobAssetReference<ParticlePropertiesBlob> Value;
}

public struct ParticlePropertiesBlob
{
	public BlobArray<float> m_lons;
    public BlobArray<float> m_lats;
	public BlobArray<float> m_depths;
}

