using Unity.Collections;
using Unity.Entities;

public struct ParticleSpawnData : IComponentData, IEnableableComponent
{
	public int m_entriesPerParticle;
	public BlobAssetReference<ParticlePropertiesBlob> Value;
}
