using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invalidURLWindow : MonoBehaviour
{
    [SerializeField]
    GameObject m_invalidURLWindow, m_setupAPIWindow;

    [SerializeField]
    CustomButton m_closeButton;

    void Awake()
    {
        m_closeButton.onClick.AddListener(CloseInvalidURLWindow);
    }

    private void CloseInvalidURLWindow()
    {
        m_invalidURLWindow.SetActive(false);
        m_setupAPIWindow.SetActive(true);
    }
}
