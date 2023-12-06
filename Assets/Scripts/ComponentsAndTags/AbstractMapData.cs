using Unity.Entities;
using Unity.Mathematics;

public struct AbstractMapData : IComponentData
{
    public double4x4 ECEFMatrix;

    public float timelineValue; // The value of the timeline (in the range 0 to 1).
}
