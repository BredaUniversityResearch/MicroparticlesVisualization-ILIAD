using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWindow : MonoBehaviour
{
    [SerializeField]
    CustomButton m_openWindowButton;

    [SerializeField]
    GameObject m_window, m_canvasOutliner;

    private void Awake()
    {
        m_openWindowButton.onClick.AddListener(() => { m_window.SetActive(true); m_canvasOutliner.SetActive(true); } );
    }
}
