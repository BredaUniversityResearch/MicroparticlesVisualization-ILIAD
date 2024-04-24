using System.Collections;
using System.Collections.Generic;
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

    // Update is called once per frame
    void Update()
    {
        if(Georeference == null || CameraGlobeAnchor == null || Label == null) return;

        // Convert the screen-space width to Unity world coordinates.
        var p0 = new Vector3((Camera.pixelWidth - ScreenSpaceWidth) / 2.0f, Camera.pixelHeight / 2.0f, Camera.nearClipPlane);
        var p1 = new Vector3((Camera.pixelWidth + ScreenSpaceWidth) / 2.0f, Camera.pixelHeight / 2.0f, Camera.nearClipPlane);
        var p2 = new Vector3(Camera.pixelWidth / 2.0f, Camera.pixelHeight / 2.0f, Camera.nearClipPlane);
        p0 = Camera.ScreenToWorldPoint(p0);
        p1 = Camera.ScreenToWorldPoint(p1);
        p2 = Camera.ScreenToWorldPoint(p2);

        // Convert to ECEF (in meters)
        var d0 = Georeference.TransformUnityPositionToEarthCenteredEarthFixed(double3(p0));
        var d1 = Georeference.TransformUnityPositionToEarthCenteredEarthFixed(double3(p1));
        var d2 = Georeference.TransformUnityPositionToEarthCenteredEarthFixed(double3(p2));

        // Get the camera's position in ECEF coordinates.
        var c = CameraGlobeAnchor.positionGlobeFixed;
        // Get the camera's height above the globe in meters.
        var h = CameraGlobeAnchor.longitudeLatitudeHeight.z;

        // TODO: Apply the law of similar triangles to compute the distance in meters.

    }
}
