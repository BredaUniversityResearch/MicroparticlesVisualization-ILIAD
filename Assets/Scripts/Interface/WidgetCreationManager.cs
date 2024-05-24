using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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

        m_latitude.onValueChanged.AddListener( OnValueChanged );
        m_longitude.onValueChanged.AddListener( OnValueChanged );
    }

    void OnEnable()
    {
        m_latitude.Select(); // This doesn't seem to work...
        EventSystem.current.SetSelectedGameObject(m_latitude.gameObject);
    }

    void Update()
    {
        // Press enter to create widget (if the data is valid and the button is interactable)
        if (Input.GetKeyDown(KeyCode.Return) && m_createWidgetButton.IsInteractable())
        {
            m_createWidgetButton.onClick.Invoke();
        }

        // Switch input fields with tab.
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (m_latitude.isFocused)
            {
                m_longitude.Select();
            }
            else
            {
                m_latitude.Select();
            }
        }
    }

    // Validate the longitude and latitude values.
    // Only enable the m_createWidgetButton when the values are valid.
    void OnValueChanged(string _)
    {
        // Validate both the latitude and longitude values.
        if (double.TryParse(m_latitude.text, out var latitude) &&
            double.TryParse(m_longitude.text, out var longitude) && 
            latitude is >= -90.0 and <= 90.0 &&
            longitude is >= -180.0 and <= 180.0)
        {
            m_createWidgetButton.interactable = true;
        }
        else
        {
            m_createWidgetButton.interactable = false;
        }
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
        m_createWidgetButton.interactable = false;
    }

    public void SetWidget(GameObject a_widget, GameObject a_widgetCreateButton)
    {
        m_widget = a_widget;
        m_widgetCreateObject = a_widgetCreateButton;
    }
}
