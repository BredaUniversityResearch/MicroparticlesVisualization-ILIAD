using Unity.Entities;
using Unity.Mathematics;

public struct AbstractMapData : IComponentData
{
    public double4x4 ECEFMatrix;
    public float3 cameraPosition; // The position of the camera (used to compute the size of the particles).
    public float cameraHeight; // The height of the camera above the map.
    public float cameraFoV; // The fov of the camera (in radians).
    public float timelineValue; // The value of the timeline (in the range 0 to 1).
    public float particleSize; // The size the particle should appear on screen.
}
