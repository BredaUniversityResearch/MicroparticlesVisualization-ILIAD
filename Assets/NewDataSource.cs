using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewDataSource : MonoBehaviour
{
    [SerializeField]
    CustomInputField m_latitude, m_longitude, m_name;

    [SerializeField]
    CustomButton m_addDataSourceButton, m_closeButton;

    [SerializeField]
    GameObject m_popOutWindow, m_canvasOutliner, m_dataSource, m_dataSourceToggle, m_dataSourceContent;

    // Start is called before the first frame update
    void Awake()
    {
        m_addDataSourceButton.onClick.AddListener(() => { CreateDataSource(); CleanUp(); });
        m_closeButton.onClick.AddListener(() => { CleanUp(); });
    }

    private void CreateDataSource()
    {
        GameObject dataSourceToggle = Instantiate(m_dataSourceToggle);
        dataSourceToggle.transform.SetParent(m_dataSourceContent.transform, false);
        dataSourceToggle.transform.SetAsFirstSibling();
        dataSourceToggle.transform.GetComponentInChildren<TMP_Text>().SetText(m_name.text);
        dataSourceToggle.GetComponent<DataSourceInfo>().m_longitude = m_longitude.text;
        dataSourceToggle.GetComponent<DataSourceInfo>().m_latitude = m_latitude.text;
    }

    private void CleanUp()
    {
        m_name.text = "";
        m_longitude.text = "";
        m_latitude.text = "";
        m_popOutWindow.SetActive(false);
        m_canvasOutliner.SetActive(false);
    }
}
