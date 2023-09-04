using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightTopBarManager : MonoBehaviour
{
    [SerializeField]
    CustomToggle /*m_externalDataToggle,*/ m_mapInformationToggle, m_legendToggle, m_timeViewToggle;

    [SerializeField]
    GameObject /*m_externalDataWindow,*/ m_mapInformationWindow, m_legendWindow, m_timeViewWindow;

    private void Awake()
    {
        //m_externalDataToggle.onValueChanged.AddListener((b) => { m_externalDataWindow.SetActive(b); });
        m_mapInformationToggle.onValueChanged.AddListener((b) => { m_mapInformationWindow.SetActive(b); });
        m_legendToggle.onValueChanged.AddListener((b) => { m_legendWindow.SetActive(b); });
        m_timeViewToggle.onValueChanged.AddListener((b) => { m_timeViewWindow.SetActive(b); });
    }
}
