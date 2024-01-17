using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEntry : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_toggleEntry;
    [SerializeField]
    CustomToggle m_infoToggle;
    [SerializeField]
    GameObject m_toggleEntryVisual, m_infoToggleVisual, m_canvasOutliner;

    private void Awake()
    {
        //TODO
        //m_toggleEntry.onValueChanged.AddListener((b) => m_toggleEntryVisual.SetActive(b));
        m_infoToggle.onValueChanged.AddListener((b) => { VisualiseInfo(b); m_canvasOutliner.SetActive(b); });
    }

    void VisualiseInfo(bool a_setActive)
    {
        m_infoToggleVisual.SetActive(a_setActive);
    }
}
