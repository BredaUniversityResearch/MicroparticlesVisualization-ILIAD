using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TableEntry : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_name, m_buoy, m_date;

    [SerializeField]
    CustomButton m_entryButton;

    string m_url;

    private void Awake()
    {
        m_entryButton.onClick.AddListener(() => { Application.OpenURL(m_url); });
    }

    public void SetContent(string a_name, string a_buoy, string a_buoyText, string a_date, string a_url)
    {
        m_name.text = a_name;
        m_buoy.text = a_buoyText;
        m_date.text = a_date;
        m_url = a_url;
    }
}
