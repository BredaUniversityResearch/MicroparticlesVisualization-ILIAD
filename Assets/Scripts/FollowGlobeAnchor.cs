using CesiumForUnity;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

/// <summary>
/// This component will update it's onw GlobeAnchor's longitude and latitude values to match the
/// longitude/latitude of a target globe anchor. The height is not sync'd with the target globe anchor.
/// </summary>
[RequireComponent(typeof(CesiumGlobeAnchor))]
public class FollowGlobeAnchor : MonoBehaviour
{

    /// <summary>
    /// The globe anchor to synchronize with this one.
    /// </summary>
    public CesiumGlobeAnchor TargetGlobeAnchor;

    /// <summary>
    /// My globe anchor to sync with the target globe anchor.
    /// </summary>
    private CesiumGlobeAnchor globeAnchor;

    // Start is called before the first frame update
    void Start()
    {
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (TargetGlobeAnchor)
        {
            double3 longitudeLatitudeHeight = TargetGlobeAnchor.longitudeLatitudeHeight;
            globeAnchor.longitudeLatitudeHeight = double3(longitudeLatitudeHeight.x, longitudeLatitudeHeight.y,
                globeAnchor.longitudeLatitudeHeight.z);
        }
    }
}
