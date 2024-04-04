using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelection : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_streetMap, m_satelliteMap, m_bathymetryMap, m_3dMap;

    [SerializeField]
    Material m_bathymetryMaterial;

    [SerializeField]
    CesiumForUnity.CesiumIonRasterOverlay m_cesiumIonRasterOverlay;

    [SerializeField]
    CesiumForUnity.Cesium3DTileset m_cesium3DTileset;


    //TODO: Logic for 3D map.
    //Are we switching the camera controller?
    private void Awake()
    {
        m_streetMap.onValueChanged.AddListener(b => OnStreetMapValueChanged(b));
        m_satelliteMap.onValueChanged.AddListener(b => OnSatelliteMapValueChanged(b));
        m_bathymetryMap.onValueChanged.AddListener(b => OnBathymetryMapValueChanged(b));
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
            if (m_cesium3DTileset.opaqueMaterial != null)
            {
                m_cesium3DTileset.opaqueMaterial = null;
            }

            SetCesiumRasterLayerID(2);
        }
    }

    private void OnStreetMapValueChanged(bool a_value)
    {
        if (a_value)
        {
            if (m_cesium3DTileset.opaqueMaterial != null)
            {
                m_cesium3DTileset.opaqueMaterial = null;
            }

            SetCesiumRasterLayerID(4);
        }
    }

    private void OnBathymetryMapValueChanged(bool a_value)
    {
        if (a_value)
        {
            m_cesium3DTileset.opaqueMaterial = m_bathymetryMaterial;
        }
    }    
}
