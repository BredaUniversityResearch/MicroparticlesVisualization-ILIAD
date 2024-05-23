using System.Collections;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// This script will populate the widgets on the home screen when particle data is available.
/// </summary>
public class PopulateWidget : MonoBehaviour
{

    [SerializeField] 
    private WidgetManager m_widgetManager;

    void Awake()
    {
        // Start a coroutine that waits until the particles are available.
        StartCoroutine(PopulateWidgetCoroutine());
    }

    IEnumerator PopulateWidgetCoroutine()
    {
        float3 longitudeLatitudeDepth = new float3();
        while (!DataLoader.Instance.GetParticlesCenterPoint(ref longitudeLatitudeDepth))
            yield return null;

        m_widgetManager.PopulateWidget(longitudeLatitudeDepth.x.ToString(CultureInfo.InvariantCulture), longitudeLatitudeDepth.y.ToString(CultureInfo.InvariantCulture));
    }
}
