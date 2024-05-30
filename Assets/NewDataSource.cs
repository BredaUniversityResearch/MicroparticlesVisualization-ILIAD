using CesiumForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewDataSource : MonoBehaviour
{
    [SerializeField]
    CustomInputField m_latitude, m_longitude, m_name;

    [SerializeField]
    CustomButton m_addDataSourceButton, m_closeButton;

    [SerializeField]
    GameObject m_popOutWindow, m_canvasOutliner, m_dataSource, m_dataSourceToggle, m_dataSourceList, m_buoyPrefab, m_mapGeoReference;

    // Start is called before the first frame update
    void Awake()
    {
        m_addDataSourceButton.onClick.AddListener(() => { CreateDataSource(); CleanUp(); });
        m_closeButton.onClick.AddListener(() => { CleanUp(); });

        m_latitude.onValueChanged.AddListener(OnValueChanged);
        m_longitude.onValueChanged.AddListener(OnValueChanged);
    }

    void Update()
    {
        // Press enter to create widget (if the data is valid and the button is interactable)
        if (Input.GetKeyDown(KeyCode.Return) && m_addDataSourceButton.IsInteractable())
        {
            m_addDataSourceButton.onClick.Invoke();
        }

        // Switch input fields with tab.
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (m_latitude.isFocused)
            {
                m_longitude.Select();
            }
            else
            {
                m_latitude.Select();
            }
        }
    }

    // Validate the longitude and latitude values.
    // Only enable the m_createWidgetButton when the values are valid.
    void OnValueChanged(string _)
    {
        // Validate both the latitude and longitude values.
        if (double.TryParse(m_latitude.text, out var latitude) &&
            double.TryParse(m_longitude.text, out var longitude) &&
            latitude is >= -90.0 and <= 90.0 &&
            longitude is >= -180.0 and <= 180.0)
        {
            m_addDataSourceButton.interactable = true;
        }
        else
        {
            m_addDataSourceButton.interactable = false;
        }
    }

    private void CreateDataSource()
    {
        GameObject dataSourceToggle = Instantiate(m_dataSourceToggle);
        dataSourceToggle.transform.SetParent(m_dataSourceList.transform, false);
        dataSourceToggle.transform.SetAsFirstSibling();
        dataSourceToggle.transform.GetComponentInChildren<TMP_Text>().SetText(m_name.text);
        
        GameObject buoy = Instantiate(m_buoyPrefab);
        buoy.transform.SetParent(m_mapGeoReference.transform, false);
        buoy.GetComponent<CesiumGlobeAnchor>().longitude = Double.Parse(m_longitude.text);
        buoy.GetComponent<CesiumGlobeAnchor>().latitude = Double.Parse(m_latitude.text);

        dataSourceToggle.GetComponent<DataSourceToggle>().m_buoyVisual = buoy;

        buoy.SetActive(false);        
    }

    private void CleanUp()
    {
        m_name.text = "";
        m_longitude.text = "";
        m_latitude.text = "";
        m_popOutWindow.SetActive(false);
        m_canvasOutliner.SetActive(false);
    }
}
