using Unity.Entities;
using Unity.Mathematics;

public struct AbstractMapData : IComponentData
{
    public double4x4 ECEFMatrix;
}
