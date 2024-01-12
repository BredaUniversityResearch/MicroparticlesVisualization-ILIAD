using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used to sync camera properties from one camera to another.
/// This is used to keep the near/far clipping planes of the particle camera in sync
/// with the world camera (which has dynamic clipping planes).
/// </summary>
public class SyncCameraProperties : MonoBehaviour
{
    public Camera sourceCamera;
    public Camera targetCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (sourceCamera != null && targetCamera != null)
        {
            targetCamera.nearClipPlane = sourceCamera.nearClipPlane;
            targetCamera.farClipPlane = sourceCamera.farClipPlane;
        }
    }
}
