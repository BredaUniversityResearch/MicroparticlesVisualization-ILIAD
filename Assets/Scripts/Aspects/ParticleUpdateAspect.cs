using System;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements;
using static Unity.Mathematics.math;

public readonly partial struct ParticleUpdateAspect : IAspect
{
    private readonly RefRW<LocalTransform> m_transform;
    private readonly RefRO<ParticleProperties> m_particleProperties;
	private readonly RefRW<ParticleColourComponent> m_colour;

    /// <summary>
    /// Get the particle longitude/latitude/depth at a specific index.
    /// </summary>
    /// <param name="index">The index in the longitude, latitude, and depth buffers.</param>
    /// <returns>The particles longitude, latitude, depth at a specific index.</returns>
    public float3 this[int index] =>
        float3(m_particleProperties.ValueRO.Value.Value.m_lons[index],
            m_particleProperties.ValueRO.Value.Value.m_lats[index],
            m_particleProperties.ValueRO.Value.Value.m_depths[index]);

    public float3 Position
    {
        get => m_transform.ValueRO.Position;
        set => m_transform.ValueRW.Position = value;
    }

    public float4 Colour
    {
        set => m_colour.ValueRW.Value = value;
    }
}
