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
	private readonly RefRW<ColourComponent> m_colourGradient;
	private readonly RefRW<DarknessComponent> m_darkness;

    /// <summary>
    /// Get the number of particles in the particles array.
    /// The length of the `m_lons` array should be the same as `m_lats` and `m_depths`.
    /// </summary>
    public int NumParticles => m_particleProperties.ValueRO.Value.Value.m_lons.Length;

    /// <summary>
    /// Get the particle longitude/latitude/depth at a specific index.
    /// </summary>
    /// <param name="index">The index in the longitude, latitude, and depth buffers.</param>
    /// <returns>The particles longitude, latitude, depth at a specific index.</returns>
    public float3 this[int index] =>
        float3(m_particleProperties.ValueRO.Value.Value.m_lons[index],
            m_particleProperties.ValueRO.Value.Value.m_lats[index],
            m_particleProperties.ValueRO.Value.Value.m_depths[index]);

    /// <summary>
    /// Get the interpolated position of the particle at a floating-point index.
    /// Note: t must be in the range [0 ... n) where n is the number of particles.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public float3 this[float t] 
    {
        get
        {
            int idx0 = (int)t;
            float alpha = t - idx0;
            int idx1 = min(idx0 + 1, NumParticles - 1);

            // alpha = (1.0f - cos(alpha * PI)) * 0.5f; // This does not produce better interpolation...

            return lerp(this[idx0], this[idx1], alpha);
        }
    }

    public float3 Position
    {
        get => m_transform.ValueRO.Position;
        set => m_transform.ValueRW.Position = value;
    }

    public float Scale
    {
        get => m_transform.ValueRO.Scale;
        set => m_transform.ValueRW.Scale = value;
    }

    public float4 Colour
    {
        set => m_colour.ValueRW.Value = value;
    }

    public float ColourGradient
    {
        set => m_colourGradient.ValueRW.Value = value;
    }

    public float Darkness
    {
        set => m_darkness.ValueRW.Value = value;
    }
}
