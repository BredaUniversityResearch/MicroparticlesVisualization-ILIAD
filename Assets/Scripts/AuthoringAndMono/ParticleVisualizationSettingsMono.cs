using Mapbox.Unity.Map;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ParticleVisualizationSettingsMono : MonoBehaviour
{
	public GameObject m_particlePrefab;

    private class Baker : Baker<ParticleVisualizationSettingsMono>
    {
        public override void Bake(ParticleVisualizationSettingsMono authoring)
        {
            var settingsEntity = GetEntity(TransformUsageFlags.None);
            AddComponent(settingsEntity, new ParticleVisualizationSettingsData
            {
                m_particlePrefab = GetEntity(authoring.m_particlePrefab, TransformUsageFlags.None),
            });
        }
    }
}
