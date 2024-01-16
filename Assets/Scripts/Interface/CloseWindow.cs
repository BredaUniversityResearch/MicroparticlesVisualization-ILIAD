using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWindow : MonoBehaviour
{
    [SerializeField]
    CustomButton m_closeWindowButton;

    [SerializeField]
    GameObject m_window, m_canvasOutliner;

    private void Awake()
    {
        m_closeWindowButton.onClick.AddListener(() => { m_canvasOutliner.SetActive(false); } );
    }
}
