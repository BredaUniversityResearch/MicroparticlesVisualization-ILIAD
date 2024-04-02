using ColourPalette;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TimeWindow : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_playToggle, m_pauseToggle;

    [SerializeField]
    CustomButton m_ffwButton;

    [SerializeField]
    AbstractMapInterface m_simulationClass;

    [SerializeField]
    Image m_timelineFillImage;

    [SerializeField]
    TextMeshProUGUI m_textSpeed;

    [SerializeField] GameObject m_loadingCover;
    [SerializeField] GameObject m_noDataCover;

    private int m_ffwSpeedIndex = 0;
    private float[] m_ffwSpeeds = { 2f, 4f, 8f };

    private void Awake()
    {
        m_playToggle.onValueChanged.AddListener(isOn => { if (isOn) PlaySimulation(); });
        m_pauseToggle.onValueChanged.AddListener(isOn => { if (isOn) PauseSimulation(); });
        m_ffwButton.onClick.AddListener(() => { IncreaseFFWSpeed(); });
        DataLoader.Instance.m_onLoadStartEvent += OnDataLoadStart;
        DataLoader.Instance.m_onLoadEndEvent += OnDataLoadEnd;
        m_loadingCover.SetActive(DataLoader.Instance.IsLoading);
    }

    void OnDataLoadStart()
    {
        m_pauseToggle.isOn = true;
        m_loadingCover.SetActive(true);
        m_noDataCover.SetActive(false);
        m_simulationClass.timelineTestValue = 0f;
    }

    void OnDataLoadEnd(bool a_success)
    {
        m_loadingCover.SetActive(false);
        if(!a_success)
            m_noDataCover.SetActive(true);
    }

    private void PlaySimulation()
    {
        m_simulationClass.play = true;
        m_simulationClass.stepRate = 0.1f;
        m_ffwSpeedIndex = 0;
        m_textSpeed.text = "";
    }
    private void PauseSimulation()
    {
        m_simulationClass.play = false;
        m_textSpeed.text = "";
        m_ffwSpeedIndex = 0;
    }
    private void IncreaseFFWSpeed()
    {
        m_playToggle.isOn = false;
        m_pauseToggle.isOn = false;

        // Implement FFW logic here, including cycling through speeds
        float currentSpeed = m_ffwSpeeds[m_ffwSpeedIndex];
        m_ffwSpeedIndex = (m_ffwSpeedIndex + 1) % m_ffwSpeeds.Length;

        // Set simulation speed to currentSpeed
        m_simulationClass.stepRate = currentSpeed / 10.0f;
        m_textSpeed.text = currentSpeed.ToString("0") + "x";
    }


    private void Update()
    {
        if (m_simulationClass.play)
        {
            m_timelineFillImage.fillAmount = m_simulationClass.timelineTestValue;
        }
    }

}
