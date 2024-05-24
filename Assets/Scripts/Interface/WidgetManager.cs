using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_widget, m_createWidget, m_canvasOutliner, m_popOutWindow;

    [SerializeField]
    CustomButton m_createWidgetButton;

    private WeatherInfo m_WeatherInfo;

    // Start is called before the first frame update
    void Awake()
    {
        m_WeatherInfo = m_widget.GetComponent<WeatherInfo>();
        m_createWidgetButton.onClick.AddListener(() => { EnablePopOut(); });
    }

    void EnablePopOut()
    {
        m_popOutWindow.SetActive(true);
        m_canvasOutliner.SetActive(true);
        m_popOutWindow.GetComponent<WidgetCreationManager>().SetWidget(m_widget, m_createWidget);
    }

    public void PopulateWidget(string longitude, string latitude)
    {
        m_WeatherInfo.WidgetCreation(longitude, latitude);
    }
}
