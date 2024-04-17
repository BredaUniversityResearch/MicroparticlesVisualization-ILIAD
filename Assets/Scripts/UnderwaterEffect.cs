using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Enable or disable the underwater effect when the camera moves below the water (height = 0).
/// </summary>
public class UnderwaterEffect : MonoBehaviour
{
    /// <summary>
    /// The globe anchor of the camera.
    /// </summary>
    public CesiumGlobeAnchor Camera;

    /// <summary>
    /// The process volume that is used to create the underwater effect.
    /// </summary>
    public Volume Volume;

    /// <summary>
    /// The minimum height of the camera (in meters above sea level) to enable the underwater effect.
    /// </summary>
    public double Height = 0.0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera != null && Volume != null)
        {
            Volume.enabled = Camera.longitudeLatitudeHeight.z < Height;
        }
    }
}
