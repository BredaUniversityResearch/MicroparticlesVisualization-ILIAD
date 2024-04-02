using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSetupWindow : MonoBehaviour
{
    [SerializeField]
    private GameObject m_firstSetupWindow, m_canvasOutlier, m_setupAPIWindow;

    [SerializeField]
    private CustomButton m_laterButton, m_closeButton, m_nextWindowButton;

    private void Awake()
    {
        m_closeButton.onClick.AddListener(OnCloseButtonPress);
        m_laterButton.onClick.AddListener(OnCloseButtonPress);
        m_nextWindowButton.onClick.AddListener(OnNextWindowButtonPress);
    }

    private void OnNextWindowButtonPress()
    {
        m_firstSetupWindow.SetActive(false);
        m_setupAPIWindow.SetActive(true);
    }

    private void OnCloseButtonPress()
    {
        m_firstSetupWindow.SetActive(false);
        m_canvasOutlier.SetActive(false);
    }
}
