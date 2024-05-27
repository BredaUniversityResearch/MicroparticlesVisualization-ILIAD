using Unity.Entities;
using Unity.Mathematics;

public struct ParticleVisualizationSettingsData : IComponentData
{
	public Entity m_particlePrefab;
	public int m_colourIndex;
	public int m_darknessIndex;
	public float4 m_sizeDepthFilter; //[sizemin, sizemax, depthmin, depthmax]
	public int m_typeFilter; //type bitmask
}