using Unity.Entities;
using Unity.Mathematics;

public struct AbstractMapData : IComponentData
{
    public float scaleFactor; // 2^(map.InitialZoom - map.AbsoluteZoom)
    public float2 centerMercator; // map.CenterMercator
    public float worldRelativeScale; // map.WorldRelativeScale

    public float3 mapPosition;
    public float3 mapScale;
    public quaternion mapRotation;
}
