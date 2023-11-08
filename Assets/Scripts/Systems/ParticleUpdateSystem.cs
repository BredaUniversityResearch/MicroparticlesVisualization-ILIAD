using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;

[BurstCompile]
public partial struct ParticlePositioningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState a_state)
    {
        a_state.RequireForUpdate<ParticleTimingData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState a_state)
    { }

    [BurstCompile]
    public void OnUpdate(ref SystemState a_state)
    {
        if (SystemAPI.HasSingleton<AbstractMapData>())
        {
            Entity particleTimingEnt = SystemAPI.GetSingletonEntity<ParticleTimingData>();
            ParticleTimingAspect particleTimingAspect = SystemAPI.GetAspect<ParticleTimingAspect>(particleTimingEnt);
            int timeIndex = particleTimingAspect.PassTime(SystemAPI.Time.DeltaTime);
            
            Entity abstractMapDataEnt = SystemAPI.GetSingletonEntity<AbstractMapData>();
            AbstractMapData abstractMapData = SystemAPI.GetComponent<AbstractMapData>(abstractMapDataEnt);

            new PositionParticleJob
            {
                TimeIndex = timeIndex,
                ScaleFactor = abstractMapData.scaleFactor,
                CenterMercator = abstractMapData.centerMercator,
                WorldRelativeScale = abstractMapData.worldRelativeScale,
                Position = abstractMapData.mapPosition,
                Scale = abstractMapData.mapScale,
                Rotation = abstractMapData.mapRotation
            }.ScheduleParallel();
        }
    }
}

[BurstCompile]
public partial struct PositionParticleJob : IJobEntity
{
    /// <summary>according to https://wiki.openstreetmap.org/wiki/Zoom_levels</summary>
    private const int EarthRadius = 6378137; //no seams with globe example
    private const double OriginShift = 2 * PI * EarthRadius / 2;

    public int TimeIndex;
    public float ScaleFactor;
    public double2 CenterMercator;
    public float WorldRelativeScale;
    public float3 Position;
    public float3 Scale;
    public quaternion Rotation;

    [BurstCompile]
    private void Execute(ParticleUpdateAspect a_particle)
    {
        var pos = a_particle[TimeIndex];

        float rg = pow(1f - abs(pos.z) / 100f, 2);
        float b = 1f - pow(abs(pos.z) / 100f, 2);

        a_particle.Colour = float4(rg, rg, b, 1f);
        a_particle.Position = GeoToWorldPosition(pos);
    }

    [BurstCompile]
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
