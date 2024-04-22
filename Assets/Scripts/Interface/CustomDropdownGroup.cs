using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomDropdownGroup : MonoBehaviour
{
    [SerializeField]
    List<CustomDropdown> m_dropdowns;

    [SerializeField]
    List<GameObject> m_legendDropdowns;

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
        if(a_dropdownValue == 0)
        {
            ClearLegendEntry(m_dropdowns.IndexOf(a_dropdown));
        }
        if (a_dropdownValue == 1) // Colour
        {
            if(m_darknessIndex == m_dropdowns.IndexOf(a_dropdown))
            {
                m_darknessIndex = -1;
                ClearLegendEntry(m_dropdowns.IndexOf(a_dropdown));
            }
            m_colourIndex = m_dropdowns.IndexOf(a_dropdown);

            UpdateLegendWindow(m_dropdowns.IndexOf(a_dropdown), a_dropdownValue);
        }
        else if (a_dropdownValue == 2) // Darkness
        {
            if (m_colourIndex == m_dropdowns.IndexOf(a_dropdown))
            {
                m_colourIndex = -1;
            }
            m_darknessIndex = m_dropdowns.IndexOf(a_dropdown);

            UpdateLegendWindow(m_dropdowns.IndexOf(a_dropdown), a_dropdownValue);
        }

        DataLoader.Instance.UpdateParticleVisualizationSettingsData(m_colourIndex, m_darknessIndex);
    }

    private void UpdateLegendWindow(int a_categoryIndex, int a_legendValue)
    {
        if(!m_legendDropdowns[a_categoryIndex].gameObject.activeSelf)
        {
            //Type/Size/Depth Dropdown
            m_legendDropdowns[a_categoryIndex].SetActive(true);

            //Colour/Darkness Dropdown
            m_legendDropdowns[a_categoryIndex].transform.GetChild(1).GetChild(a_legendValue - 1).gameObject.SetActive(true);
        }
        else
        {
            m_legendDropdowns[a_categoryIndex].transform.GetChild(1).GetChild(a_legendValue - 1).gameObject.SetActive(true);

            switch(a_legendValue)
            {
                case 1:
                    m_legendDropdowns[a_categoryIndex].transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                    break;
                case 2:
                    m_legendDropdowns[a_categoryIndex].transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }        
    }

    private void ClearLegendEntry(int a_categoryIndex)
    {
        m_legendDropdowns[a_categoryIndex].transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        m_legendDropdowns[a_categoryIndex].transform.GetChild(1).GetChild(1).gameObject.SetActive(false);

        m_legendDropdowns[a_categoryIndex].SetActive(false);
    }
}
