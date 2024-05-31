using System.Globalization;
using Unity.Mathematics;
using UnityEngine;

public class WidgetManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_widget, m_createWidget, m_canvasOutliner, m_popOutWindow;

    [SerializeField]
    CustomButton m_createWidgetButton;

    [SerializeField]
    CustomToggle m_homePageToggle;

    private WeatherInfo m_WeatherInfo;

    // Start is called before the first frame update
    void Awake()
    {
        m_WeatherInfo = m_widget.GetComponent<WeatherInfo>();
        m_createWidgetButton.onClick.AddListener(() => { EnablePopOut(); });
        m_homePageToggle?.onValueChanged.AddListener((isOn) => {
            if (isOn)
            {
                PopulateWidget();
            }
        });
    }

    void EnablePopOut()
    {
        m_popOutWindow.SetActive(true);
        m_canvasOutliner.SetActive(true);
        m_popOutWindow.GetComponent<WidgetCreationManager>().SetWidget(m_widget, m_createWidget);
    }

    public void PopulateWidget()
    {
        float3 longitudeLatitudeHeight = float3.zero;
        if (DataLoader.Instance.GetParticlesCenterPoint(ref longitudeLatitudeHeight))
        {
            PopulateWidget(longitudeLatitudeHeight.x.ToString(CultureInfo.InvariantCulture), longitudeLatitudeHeight.y.ToString(CultureInfo.InvariantCulture));
        }
    }

    public void PopulateWidget(string longitude, string latitude)
    {
        m_createWidget.SetActive(false);
        m_widget.SetActive(true);
        m_WeatherInfo.WidgetCreation(longitude, latitude);
    }
}
