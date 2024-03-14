using CesiumForUnity;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

/// <summary>
/// This component will respond to the `Cesium3DTileset.OnTileGameObjectCreated` action
/// and add the longitude, latitude, and height of the vertex to the UV1 texture coordinate
/// channel of the mesh for the tile.
/// This is required for adding the elevation contour lines and an elevation ramp to
/// the terrain tileset.
/// </summary>
public class AddHeightTiles : MonoBehaviour
{
    /// <summary>
    /// The Cesium 3D Tileset component.
    /// </summary>
    public Cesium3DTileset Tileset;

    /// <summary>
    /// The Georeference required to convert to Lat/Lon/Height coordinates.
    /// </summary>
    public CesiumGeoreference Georeference;

    public void Awake()
    {
        if (Tileset)
        {
            Tileset.OnTileGameObjectCreated += Tileset_OnTileGameObjectCreated;
        }
    }

    private void Tileset_OnTileGameObjectCreated(GameObject go)
    {
        foreach (var meshFilter in go.GetComponentsInChildren<MeshFilter>())
        {
            var localToWorldMatrix = meshFilter.transform.localToWorldMatrix;
            var mesh = meshFilter.sharedMesh;
            var meshDataArray = Mesh.AcquireReadOnlyMeshData(mesh);
            var meshData = meshDataArray[0];

            // Allocate storage for vertex and height data.
            var verts = new NativeArray<Vector3>(meshData.vertexCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            // The resulting longitude, latitude, and height values.
            var lonLatHeight = new NativeArray<Vector3>(meshData.vertexCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            // Get vertex positions. 
            meshData.GetVertices(verts);

            for (int i = 0; i < verts.Length; i++)
            {
                var v3 = localToWorldMatrix.MultiplyPoint3x4(verts[i]);
                var d3 = double3(v3);
                // Convert vertex to ECEF.
                d3 = Georeference.TransformUnityPositionToEarthCenteredEarthFixed(d3);
                // Convert to Longitude, Latitude, and Height.
                d3 = CesiumWgs84Ellipsoid.EarthCenteredEarthFixedToLongitudeLatitudeHeight(d3);
                // Store Lon, Lat, Height values.
                lonLatHeight[i] = new Vector3((float)d3.x, (float)d3.y, (float)d3.z);
            }

            //new ComputeHeight
            //{
            //    Vertices = verts,
            //    LonLatHeight = lonLatHeight,
            //    LocalToECEF = Georeference.localToEcefMatrix
            //}.Schedule(meshData.vertexCount, 64).Complete();

            // Calling Mesh.AcquireReadOnlyMeshData does not cause any memory
            // allocations or data copies by default, as long as you dispose
            // of the MeshDataArray before modifying the Mesh.
            // However, if you call Mesh.AcquireReadOnlyMeshData and then modify
            // the Mesh while the MeshDataArray exists, Unity must copy the
            // MeshDataArray into a new memory allocation.
            // Source: https://docs.unity3d.com/ScriptReference/Mesh.MeshData.html
            meshDataArray.Dispose();

            // Assign the LonLatHeight data to the mesh's UV3 texture coords.
            mesh.SetUVs(3, lonLatHeight);

            lonLatHeight.Dispose();
            verts.Dispose();
        }
    }

}

[BurstCompile]
struct ComputeHeight : IJobParallelFor
{
    // Vertex positions in Unity coordinates.
    [ReadOnly] public NativeArray<Vector3> Vertices;

    // Longitude, Latitude and Height of each coordinate.
    [WriteOnly] public NativeArray<Vector3> LonLatHeight;

    /// <summary>
    /// Matrix to convert from Unity coords to ECEF.
    /// </summary>
    public double4x4 LocalToECEF;

    static double4 ToDouble4(Vector3 v, double w = 1.0)
    {
        return double4(v.x, v.y, v.z, w);
    }

    static Vector3 ToVector3(double3 v)
    {
        return new Vector3((float)v.x, (float)v.y, (float)v.z);
    }

    public void Execute(int i)
    {
        var pos = ToDouble4(Vertices[i]);
        var ecef = mul(LocalToECEF, pos).xyz;

        var lonLatHeight = CesiumWgs84Ellipsoid.EarthCenteredEarthFixedToLongitudeLatitudeHeight(ecef);

        LonLatHeight[i] = ToVector3(lonLatHeight);
    }
}
