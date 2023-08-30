using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftTopBarManager : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_homepageToggle, m_measurementsToggle, m_mapToggle, m_alertsToggle, m_settingsToggle;

    [SerializeField]
    GameObject m_homepageScreen, m_measurementsScreen, m_mapScreen, m_alertsScreen, m_settingsScreen, m_rightTopBar, m_homepageItemToHide, m_homepageItemToShow;

    [SerializeField]
    CustomButton m_collapseLeftBar, m_expandLeftBar;

    private void Awake()
    {
        m_homepageToggle.onValueChanged.AddListener((b) => { m_homepageScreen.SetActive(b); m_rightTopBar.SetActive(!b); ExpandBar(); HomePageBar(); });
        m_measurementsToggle.onValueChanged.AddListener((b) => { m_measurementsScreen.SetActive(b); m_rightTopBar.SetActive(!b); ExpandBar(); CommonPageBar(); });
        m_mapToggle.onValueChanged.AddListener((b) => { m_mapScreen.SetActive(b); m_rightTopBar.SetActive(b); ExpandBar(); CommonPageBar(); });
        m_alertsToggle.onValueChanged.AddListener((b) => { m_alertsScreen.SetActive(b); m_rightTopBar.SetActive(!b); ExpandBar(); CommonPageBar(); });
        m_settingsToggle.onValueChanged.AddListener((b) => { m_settingsScreen.SetActive(b); m_rightTopBar.SetActive(!b); CollapseBar(); });
    }

    private void CollapseBar()
    {
        if(m_collapseLeftBar.isActiveAndEnabled)
        {
            m_collapseLeftBar.onClick.Invoke();
        }
    }

    private void ExpandBar()
    {
        if(m_expandLeftBar.isActiveAndEnabled)
        {
            m_expandLeftBar.onClick.Invoke();
        }
    }

    private void HomePageBar()
    {
        m_homepageItemToHide.SetActive(false);
        m_homepageItemToShow.SetActive(true);
    }

    private void CommonPageBar()
    {
        m_homepageItemToShow.SetActive(false);
        m_homepageItemToHide.SetActive(true);
    }

}
