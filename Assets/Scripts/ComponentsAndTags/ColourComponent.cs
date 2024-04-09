using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_ColourValue")]
public struct ColourComponent : IComponentData
{
    public float Value;
}