using UnityEngine;
using UnityEngine.UI;

public class ToggleManagement : MonoBehaviour
{
    [SerializeField] private GameObject m_typeToggleContent, m_sizeToggleContent, m_depthToggleContent;

    private void Awake()
    {
        // Register event listeners for toggles
        foreach (CustomToggle toggle in m_typeToggleContent.GetComponentsInChildren<CustomToggle>())
        {
            if(toggle.group == null)
            {
                toggle.onValueChanged.AddListener(OnToggleTypeValueChanged);
            }
        }

        foreach (CustomToggle toggle in m_sizeToggleContent.GetComponentsInChildren<CustomToggle>())
        {
            if(toggle.group == null)
            {
                toggle.onValueChanged.AddListener(OnToggleSizeValueChanged);
            }
        }

        foreach (CustomToggle toggle in m_depthToggleContent.GetComponentsInChildren<CustomToggle>())
        {
            if(toggle.group == null)
            {
                toggle.onValueChanged.AddListener(OnToggleDepthValueChanged);
            }
        }
    }

    private void OnToggleTypeValueChanged(bool isOn)
    {
        if (isOn)
        {
            // Deactivate toggles in other groups
            DeactivateToggles(m_sizeToggleContent);
            DeactivateToggles(m_depthToggleContent);
        }
    }
    private void OnToggleSizeValueChanged(bool isOn)
    {
        if (isOn)
        {
            // Deactivate toggles in other groups
            DeactivateToggles(m_typeToggleContent);
            DeactivateToggles(m_depthToggleContent);
        }
    }
    private void OnToggleDepthValueChanged(bool isOn)
    {
        if (isOn)
        {
            // Deactivate toggles in other groups
            DeactivateToggles(m_sizeToggleContent);
            DeactivateToggles(m_typeToggleContent);
        }
    }

    private void DeactivateToggles(GameObject toggleGroup)
    {
        foreach (CustomToggle toggle in toggleGroup.GetComponentsInChildren<CustomToggle>())
        {
                toggle.isOn = false;
        }
    }
}
