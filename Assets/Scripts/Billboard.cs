using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotate the game object to always face the camera.
/// </summary>
public class Billboard : MonoBehaviour
{

    public Camera Camera;
    /// <summary>
    /// The scale of the billboard is based on the distance to the camera.
    /// Use this value to scale the sprite to the desired size.
    /// </summary>
    public float DistanceScale = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        if(Camera==null)
            Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.transform);

        float d = (Camera.transform.position - transform.position).magnitude;
        transform.localScale = new Vector3(d, d, d) * DistanceScale;
    }
}
