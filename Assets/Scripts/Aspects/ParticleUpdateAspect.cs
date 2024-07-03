using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using static Unity.Mathematics.math;

public readonly partial struct ParticleUpdateAspect : IAspect
{
    private readonly RefRW<LocalTransform> m_transform;
    private readonly RefRO<ParticleProperties> m_particleProperties;
    private readonly RefRW<ParticleColourComponent> m_colour;
    private readonly RefRW<ColourComponent> m_colourGradient;
    private readonly RefRW<DarknessComponent> m_darkness;
	private readonly EnabledRefRW<MaterialMeshInfo> m_meshInfo;

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
    /// Note: t must be in the range [0 ... n) where n is the number of indices.
    /// </summary>
    /// <param name="t">The floating-point index in the range [0 ... n).</param>
    /// <returns>The interpolated position of the particle.</returns>
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

    public float ParticleSize
    {
        get => m_particleProperties.ValueRO.Value.Value.m_sizes[0];
    }

    public int ParticleType
    {
        get => m_particleProperties.ValueRO.Value.Value.m_types[0];
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

	public void ApplyFilter(float a_time, float4 a_sizeDepthFilter, int a_typeFilter)
	{
		int idx0 = (int)a_time;
		float alpha = a_time - idx0;
		int idx1 = min(idx0 + 1, NumParticles - 1);
        float depthAtT = lerp(m_particleProperties.ValueRO.Value.Value.m_depths[idx0],
            m_particleProperties.ValueRO.Value.Value.m_depths[idx1],
            alpha);

        bool filter = a_sizeDepthFilter[2] <= depthAtT && a_sizeDepthFilter[3] >= depthAtT
            && a_sizeDepthFilter[0] <= ParticleSize && a_sizeDepthFilter[1] >= ParticleSize &&
            (a_typeFilter & (1 << ParticleType)) != 0;
        m_meshInfo.ValueRW = filter;
	}
}
