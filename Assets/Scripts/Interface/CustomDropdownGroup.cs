using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomDropdownGroup : MonoBehaviour
{
    [SerializeField]
    List<CustomDropdown> m_dropdowns;

    public int m_colourIndex, m_darknessIndex = -1;

    private void Awake()
    {
        foreach (CustomDropdown dropdown in m_dropdowns)
        {
            dropdown.onValueChanged.AddListener((b) => { CheckDropdowns(b, dropdown); UpdateIndices(b, dropdown); });
        }
    }

    public void CheckDropdowns(int a_dropdownValue, CustomDropdown a_dropdown)
    {
        foreach (CustomDropdown otherDropdown in m_dropdowns)
        {
            if (otherDropdown != a_dropdown)
            {
                if(a_dropdownValue == otherDropdown.value)
                {
                    otherDropdown.value = 0;
                }
            }
        }
    }

    public void UpdateIndices(int a_dropdownValue, CustomDropdown a_dropdown)
    {
        if (a_dropdownValue == 1) // Colour
        {
            if(m_darknessIndex == m_dropdowns.IndexOf(a_dropdown))
            {
                m_darknessIndex = -1;
            }
            m_colourIndex = m_dropdowns.IndexOf(a_dropdown);
        }
        else if (a_dropdownValue == 2) // Darkness
        {
            if (m_colourIndex == m_dropdowns.IndexOf(a_dropdown))
            {
                m_colourIndex = -1;
            }
            m_darknessIndex = m_dropdowns.IndexOf(a_dropdown);
        }

        DataLoader.Instance.UpdateParticleVisualizationSettingsData(m_colourIndex, m_darknessIndex);
    }
}
