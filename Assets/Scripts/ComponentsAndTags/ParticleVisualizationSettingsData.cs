using Unity.Entities;

public struct ParticleVisualizationSettingsData : IComponentData
{
	public Entity m_particlePrefab;
	public int m_colourIndex;
	public int m_darknessIndex;
}