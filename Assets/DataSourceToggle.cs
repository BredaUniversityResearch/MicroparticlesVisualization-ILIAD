using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSourceToggle : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_toggleEntry;

    public GameObject m_buoyVisual;

    private void Awake()
    {
        m_toggleEntry.onValueChanged.AddListener((b) => m_buoyVisual.SetActive(b));
    }
}
