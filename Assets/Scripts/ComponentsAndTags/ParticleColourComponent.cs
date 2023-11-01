using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("ParticleColour")]
public struct ParticleColourComponent : IComponentData
{
	public float4 Value;
}