using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelection : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_streetMap, m_satelliteMap, m_3dMap;

    [SerializeField]
    CesiumForUnity.CesiumIonRasterOverlay m_cesiumIonRasterOverlay;


    //TODO: Logic for 3D map.
    //Are we switching the camera controller?
    private void Awake()
    {
        m_streetMap.onValueChanged.AddListener(b => OnStreetMapValueChanged(b));
        m_satelliteMap.onValueChanged.AddListener(b => OnSatelliteMapValueChanged(b));
        //m_3dMap.onValueChanged.AddListener(On3dMapValueChanged);
    }

    private void SetCesiumRasterLayerID(int a_layerID)
    {
        m_cesiumIonRasterOverlay.ionAssetID = a_layerID;
    }
    private void OnSatelliteMapValueChanged(bool a_value)
    {
        if (a_value)
        {
            SetCesiumRasterLayerID(2);
        }
    }

    private void OnStreetMapValueChanged(bool a_value)
    {
        if (a_value)
        {
            SetCesiumRasterLayerID(4);
        }
    }

    
}
