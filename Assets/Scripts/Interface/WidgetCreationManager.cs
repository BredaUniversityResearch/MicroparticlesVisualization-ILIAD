using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetCreationManager : MonoBehaviour
{
    [SerializeField]
    CustomInputField m_latitude, m_longitude;

    [SerializeField]
    CustomButton m_createWidgetButton, m_closeButton;

    [SerializeField]
    GameObject m_popOutWindow, m_canvasOutliner, m_widget, m_widgetCreateObject;

    // Start is called before the first frame update
    void Awake()
    {
        m_createWidgetButton.onClick.AddListener(() => { CreateWidget(); CleanUp(); });
        m_closeButton.onClick.AddListener(() => { CleanUp(); });
    }

    private void CreateWidget()
    {
        m_widget.SetActive(true);
        m_widget.GetComponent<WeatherInfo>().WidgetCreation(m_longitude.text, m_latitude.text);
        m_widgetCreateObject.SetActive(false);
    }

    private void CleanUp()
    {
        m_longitude.text = "";
        m_latitude.text = "";
        m_popOutWindow.SetActive(false);
        m_canvasOutliner.SetActive(false);
    }

    public void SetWidget(GameObject a_widget, GameObject a_widgetCreateButton)
    {
        m_widget = a_widget;
        m_widgetCreateObject = a_widgetCreateButton;
    }
}
