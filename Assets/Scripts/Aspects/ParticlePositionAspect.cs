using System;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements;
using static Unity.Mathematics.math;

public readonly partial struct ParticlePositionAspect : IAspect
{
    public readonly Entity Entity;
    public const float m_xyScale = 5f;
    public const float m_zScale = 0.1f;

    private readonly RefRW<LocalTransform> m_transform;
    private readonly RefRO<ParticleProperties> m_particleProperties;

    /// <summary>
    /// Get the particle longitude/latitude/depth at a specific index.
    /// </summary>
    /// <param name="index">The index in the longitude, latitude, and depth buffers.</param>
    /// <returns>The particles longitude, latitude, depth at a specific index.</returns>
    private float3 this[int index] =>
        float3(m_particleProperties.ValueRO.Value.Value.m_lons[index],
            m_particleProperties.ValueRO.Value.Value.m_lats[index],
            m_particleProperties.ValueRO.Value.Value.m_depths[index]);

    private float3 Position
    {
        get => m_transform.ValueRO.Position;
        set => m_transform.ValueRW.Position = value;
    }

    public void SetTimeIndex(int a_index)
    {
        float3 offset = float3(10.476f, 63.583f, 0.0f);
        float3 scale = float3(m_xyScale, m_xyScale, m_zScale);

        Position = (this[a_index] - offset) * scale;
    }

    /// <summary>
    /// Convert the longitude/latitude/depth values to Unity world space coordinates.
    /// </summary>
    /// <param name="a_index"></param>
    public void SetWordPos(int a_index)
    {

    }
}
