using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CesiumForUnity;
using TMPro;
using UnityEngine;
using static Unity.Mathematics.math;

/// <summary>
/// Computes the world scale based on a screen-space width.
/// This is used to compute the distance on the map that is
/// represented by a scale UI element where the UI elements
/// screen-space width is known.
/// </summary>
public class MapScale : MonoBehaviour
{

    /// <summary>
    /// The top-level georeference. Used to convert Unity
    /// coordinates to meters in (globe) world coordinates.
    /// </summary>
    public CesiumGeoreference Georeference;

    /// <summary>
    /// The globe anchor of the camera. This is used to compute the
    /// relative distance to the surface of the earth in world-coordinates.
    /// </summary>
    public CesiumGlobeAnchor CameraGlobeAnchor;

    /// <summary>
    /// Unity scene camera. Used to convert screen-space values to view space.
    /// </summary>
    public Camera Camera;

    /// <summary>
    /// The width of the UI element in screen-space.
    /// </summary>
    public float ScreenSpaceWidth;

    /// <summary>
    /// The UI Text element to update with the scale value.
    /// </summary>
    public TextMeshProUGUI Label;

    // Start is called before the first frame update
    void Start()
    {
        if (Camera == null)
        {
            Camera = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (Georeference == null || CameraGlobeAnchor == null || Label == null) return;

        var d = GetDistanceAtSurface();

        Label.text = Distance(d);
    }

    /// <summary>
    /// Convert a value in meters to a human-readable value.
    /// </summary>
    /// <param name="meters">The value in meters to convert.</param>
    /// <returns>The human-readable value.</returns>
    private string Distance(double meters, string format = "F1") =>
        Math.Abs(meters) switch
        {
            < 1000.0 => meters.ToString(format) + " m",
            (>= 1000.0) and (< 1000000.0) => (meters / 1000.0).ToString(format) + " km",
            (>= 1000000.0) and (< 1000000000.0) => (meters / 1000000.0).ToString(format) + " Mm",
            (>= 1000000000.0) => (meters / 1000000000.0).ToString(format) + " Gm",
            _ => "NaN"
        };

    private double GetDistanceAtSurface()
    {
        // Compute screen-space positions
        var p0 = new Vector3(Camera.pixelWidth / 2.0f, Camera.pixelHeight / 2.0f, 1.0f); // Center point on screen, 1 unit in front of the camera.
        var p1 = new Vector3((Camera.pixelWidth + ScreenSpaceWidth) / 2.0f, Camera.pixelHeight / 2.0f, 1.0f); // 1/2 from width of UI element.
        // Convert to world space.
        p0 = Camera.ScreenToWorldPoint(p0);
        p1 = Camera.ScreenToWorldPoint(p1);

        // Convert to ECEF (in meters)
        var a = Georeference.TransformUnityPositionToEarthCenteredEarthFixed(double3(p0));
        var b = Georeference.TransformUnityPositionToEarthCenteredEarthFixed(double3(p1));
        // Get the camera's position in ECEF coordinates.
        var c = CameraGlobeAnchor.positionGlobeFixed;

        // Compute distance from a -> and a -> c.
        var ab = distance(a, b);
        var ac = distance(a, c);

        // Get the camera's height above the globe in meters.
        var h = CameraGlobeAnchor.longitudeLatitudeHeight.z;

        // Distance in meters at the surface of the WGS84 ellipsoid.
        // Multiply by 2 because the original values only measure 1/2 of the triangle.
        return h * ab / ac * 2.0;
    }
}
