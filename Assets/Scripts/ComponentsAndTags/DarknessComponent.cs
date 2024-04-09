using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_Darkness")]
public struct DarknessComponent : IComponentData
{
    public float Value;
}