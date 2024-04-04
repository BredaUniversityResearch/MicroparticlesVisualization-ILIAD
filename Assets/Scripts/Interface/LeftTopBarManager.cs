using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftTopBarManager : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_homepageToggle, m_measurementsToggle, m_mapToggle, m_alertsToggle, m_settingsToggle;

    [SerializeField]
    GameObject m_homepageScreen, m_measurementsScreen, m_mapScreen, m_alertsScreen, m_settingsScreen, m_rightTopBar, m_homepageItemToHide, m_lip;

    [SerializeField]
    CollapseLeftBar m_leftBar;

    private void Awake()
    {
        m_homepageToggle.onValueChanged.AddListener((b) => { m_homepageScreen.SetActive(b); m_rightTopBar.SetActive(!b); HomePageBar(); m_lip.SetActive(false); });
        m_measurementsToggle.onValueChanged.AddListener((b) => { m_measurementsScreen.SetActive(b); m_rightTopBar.SetActive(!b); m_leftBar.Collapse(); m_lip.SetActive(false); });
        m_mapToggle.onValueChanged.AddListener((b) => { m_mapScreen.SetActive(b); m_rightTopBar.SetActive(b); MapPageBar(); m_lip.SetActive(true); });
        m_alertsToggle.onValueChanged.AddListener((b) => { m_alertsScreen.SetActive(b); m_rightTopBar.SetActive(!b); m_leftBar.Collapse(); m_lip.SetActive(false); });
        m_settingsToggle.onValueChanged.AddListener((b) => { m_settingsScreen.SetActive(b); m_rightTopBar.SetActive(!b); m_leftBar.Collapse(); m_lip.SetActive(false); });
    }

    private void HomePageBar()
    {
        m_homepageItemToHide.SetActive(false);

        m_leftBar.Expand();
    }

    private void MapPageBar()
    {
        m_homepageItemToHide.SetActive(true);

        m_leftBar.Expand();
    }

}
