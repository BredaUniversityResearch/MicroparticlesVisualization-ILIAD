using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEntry : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_toggleEntry;
    [SerializeField]
    CustomButton m_infoButton;
    [SerializeField]
    GameObject m_toggleEntryVisual, m_infoButtonVisual;

    private void Awake()
    {
        //TODO
        //m_toggleEntry.onValueChanged.AddListener((b) => m_toggleEntryVisual.SetActive(b));
        //m_infoButton.onClick.AddListener(VisualiseInfo);
    }

    void VisualiseInfo()
    {
        //TODO
    }
}
