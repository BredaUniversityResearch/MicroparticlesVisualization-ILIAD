using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ParticleVisualizationSettingsMono : MonoBehaviour
{
	public GameObject m_particlePrefab;
}

public class ParticleVisualizationSettingsBaker : Baker<ParticleVisualizationSettingsMono>
{
	public override void Bake(ParticleVisualizationSettingsMono authoring)
	{
		var settingsEntity = GetEntity(TransformUsageFlags.Dynamic);

		AddComponent(settingsEntity, new ParticleVisualizationSettingsData
		{
			m_particlePrefab = GetEntity(authoring.m_particlePrefab, TransformUsageFlags.Dynamic)
		});
	}
}
