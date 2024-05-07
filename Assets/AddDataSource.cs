using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDataSource : MonoBehaviour
{
    [SerializeField]
    CustomButton m_addDataSourceButton;

    [SerializeField]
    GameObject m_addDataSourceWindow, m_canvasOutlier;

    private void Awake()
    {
        m_addDataSourceButton.onClick.AddListener(AddData);
    }

    private void AddData()
    {
        m_canvasOutlier.SetActive(true);
        m_addDataSourceWindow.SetActive(true);
    }
}
