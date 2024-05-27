using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class ParticleVisualizationSettingsMono : MonoBehaviour
{
    public GameObject m_particlePrefab;
    public GameObject m_visualizationFilterPrefab;

    private class Baker : Baker<ParticleVisualizationSettingsMono>
    {
        public override void Bake(ParticleVisualizationSettingsMono authoring)
        {
            var settingsEntity = GetEntity(TransformUsageFlags.None);
            var customDropdownGroup = authoring.m_visualizationFilterPrefab.GetComponent<CustomDropdownGroup>();

            AddComponent(settingsEntity, new ParticleVisualizationSettingsData
            {
                m_particlePrefab = GetEntity(authoring.m_particlePrefab, TransformUsageFlags.None),
                m_colourIndex = customDropdownGroup.m_colourIndex,
                m_darknessIndex = customDropdownGroup.m_darknessIndex,
                m_sizeDepthFilter = new float4(float.NegativeInfinity, float.PositiveInfinity, -150f, 0f),
                m_typeFilter = int.MaxValue
            });
        }
    }
}