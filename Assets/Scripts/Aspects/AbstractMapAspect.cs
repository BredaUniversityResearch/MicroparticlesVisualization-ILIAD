using Mapbox.Utils;
using System.Drawing;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;
using static Unity.Mathematics.math;

/// <summary>
/// This aspect works with the AbstractMapData to mimic the AbstractMap class from the Mapbox plugin.
/// </summary>
[BurstCompile]
public readonly partial struct AbstractMapAspect : IAspect
{
    /// <summary>according to https://wiki.openstreetmap.org/wiki/Zoom_levels</summary>
    private const int EarthRadius = 6378137; //no seams with globe example
    private const double OriginShift = 2 * PI * EarthRadius / 2;

    private readonly Entity m_mapEntity;
    private readonly RefRO<AbstractMapData> m_abstractMapData;

    public float ScaleFactor => m_abstractMapData.ValueRO.scaleFactor;

    public float2 CenterMercator => m_abstractMapData.ValueRO.centerMercator;

    public float WorldRelativeScale => m_abstractMapData.ValueRO.worldRelativeScale;

    public float3 Position => m_abstractMapData.ValueRO.mapPosition;

    public float3 Scale => m_abstractMapData.ValueRO.mapScale;

    public quaternion Rotation => m_abstractMapData.ValueRO.mapRotation;

    public float3 TransformPoint(float3 p)
    {
        return Position + rotate(Rotation, p) * Scale;
    }

    [BurstCompile]
    public float3 GeoToWorldPosition(float3 latitudeLongitudeDepth)
    {
        var worldPos = GeoToWorldPositionXZ(latitudeLongitudeDepth.xy, latitudeLongitudeDepth.z);
        return TransformPoint(worldPos);
    }

    [BurstCompile]
    public float3 GeoToWorldPositionXZ(float2 latitudeLongitude, float depth)
    {
        var worldPosXY = GeoToWorldPosition(double2(latitudeLongitude), CenterMercator, WorldRelativeScale * ScaleFactor);
        return float3((float)worldPosXY.x, depth * WorldRelativeScale * ScaleFactor, (float)worldPosXY.y);
    }

    [BurstCompile]
    public double2 GeoToWorldPosition(double2 latLong, double2 refPoint, float scale = 1.0f)
    {
        return GeoToWorldPosition(latLong.x, latLong.y, refPoint, scale);
    }

    /// <summary>
    /// Converts WGS84 lat/lon to x/y meters in reference to a center point.
    /// </summary>
    /// <param name="lat">The latitude.</param>
    /// <param name="lon">The longitude.</param>
    /// <param name="refPoint">A center point to offset resultant xy, this is usually map's center mercator.</param>
    /// <param name="scale">Scale in meters. (default scale = 1)</param>
    /// <returns>A xy tile ID. </returns>
    /// <example>
    /// Converts a Lat/Lon of (37.7749, 122.4194) into Unity coordinates for a map centered at (10,10) and a scale of 2.5 meters for every 1 Unity unit 
    /// <code>
    /// var worldPosition = Conversions.GeoToWorldPosition(37.7749, 122.4194, math.double2(10, 10), 2.5f);
    /// // worldPosition = ( 11369163.38585, 34069138.17805 )
    /// </code>
    /// </example>
    [BurstCompile]
    public double2 GeoToWorldPosition(double lat, double lon, double2 refPoint, float scale = 1.0f)
    {
        var pos = LatLonToMeters(lat, lon);
        return (pos - refPoint) * scale;
    }

    /// <summary>
    /// Converts WGS84 lat/lon to Spherical Mercator EPSG:900913 xy meters.
    /// SOURCE: http://stackoverflow.com/questions/12896139/geographic-coordinates-converter.
    /// </summary>
    /// <param name="lat">The latitude.</param>
    /// <param name="lon">The longitude.</param>
    /// <returns>A double2 of xy meters.</returns>
    [BurstCompile]
    public double2 LatLonToMeters(double lat, double lon)
    {
        var posx = lon * OriginShift / 180;
        var posy = log(tan((90 + lat) * PI / 360)) / (PI / 180);
        posy = posy * OriginShift / 180;

        return double2(posx, posy);
    }
}