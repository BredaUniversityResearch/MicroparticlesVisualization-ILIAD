using ColourPalette;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class TimeWindow : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_playToggle, m_pauseToggle;

    [SerializeField]
    CustomButton m_ffwButton;

    [SerializeField]
    AbstractMapInterface m_simulationClass;

    [SerializeField]
    RectTransform m_timelineImageTransform;

    [SerializeField]
    TextMeshProUGUI m_textSpeed;

    private float m_timelineDefault = 440.0f;
    private int m_ffwSpeedIndex = 0;
    private float[] m_ffwSpeeds = { 1.5f, 2.0f, 3.0f };

    private void Awake()
    {
        m_playToggle.onValueChanged.AddListener(isOn => { if (isOn) PlaySimulation(); });
        m_pauseToggle.onValueChanged.AddListener(isOn => { if (isOn) PauseSimulation(); });
        m_ffwButton.onClick.AddListener(() => { IncreaseFFWSpeed(); });
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
        m_textSpeed.text = currentSpeed.ToString("0.0") + "x";
    }


    private void Update()
    {
        if (m_simulationClass.play)
        {
            float barWidth = m_timelineDefault * m_simulationClass.timelineTestValue;
            m_timelineImageTransform.sizeDelta = new ( barWidth, m_timelineImageTransform.sizeDelta.y);
        }
    }

}
