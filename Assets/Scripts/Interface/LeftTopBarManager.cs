using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftTopBarManager : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_homepageToggle, m_measurementsToggle, m_mapToggle, m_alertsToggle, m_settingsToggle;

    [SerializeField]
    GameObject m_homepageScreen, m_measurementsScreen, m_mapScreen, m_alertsScreen, m_settingsScreen, m_rightTopBar;

    private void Awake()
    {
        m_homepageToggle.onValueChanged.AddListener((b) => { m_homepageScreen.SetActive(b); m_rightTopBar.SetActive(!b); });
        m_measurementsToggle.onValueChanged.AddListener((b) => { m_measurementsScreen.SetActive(b); m_rightTopBar.SetActive(!b); });
        m_mapToggle.onValueChanged.AddListener((b) => { m_mapScreen.SetActive(b); m_rightTopBar.SetActive(b); });
        m_alertsToggle.onValueChanged.AddListener((b) => { m_alertsScreen.SetActive(b); m_rightTopBar.SetActive(!b); });
        m_settingsToggle.onValueChanged.AddListener((b) => { m_settingsScreen.SetActive(b); m_rightTopBar.SetActive(!b); });
    }
}
