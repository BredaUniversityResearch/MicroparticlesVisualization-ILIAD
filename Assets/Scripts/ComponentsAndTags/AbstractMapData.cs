using Unity.Entities;
using Unity.Mathematics;

public struct AbstractMapData : IComponentData
{
    public float scaleFactor; // 2^(map.InitialZoom - map.AbsoluteZoom)
    public float worldRelativeScale; // map.WorldRelativeScale
    public double2 centerMercator; // map.CenterMercator

    public float3 mapPosition;
    public float3 mapScale;
    public quaternion mapRotation;
}
