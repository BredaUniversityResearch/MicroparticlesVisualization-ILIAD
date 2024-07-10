using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomDropdownGroup : MonoBehaviour
{
    [SerializeField]
    List<CustomDropdown> m_dropdowns;

    [SerializeField] LegendDisplay m_colourLegend;
    [SerializeField] LegendDisplay m_darknessLegend;

    public int m_colourIndex = -1;
    public int m_darknessIndex = -1;
    bool m_ignoreCallback;

    private void Awake()
    {
        for(int i = 0; i < m_dropdowns.Count; i++)
        {
            int index = i;
			m_dropdowns[i].onValueChanged.AddListener((b) => 
            { 
                UpdateIndices(b, index); 
            });
        }
    }

    public void UpdateIndices(int a_dropdownValue, int a_dropdownIndex)
    {
        if (m_ignoreCallback)
            return;

        if(a_dropdownValue == 0)
        {
			if (m_colourIndex == a_dropdownIndex)
			{
				m_colourIndex = -1;
			}
			if (m_darknessIndex == a_dropdownIndex)
			{
				m_darknessIndex = -1;
			}
        }
        if (a_dropdownValue == 1) // Colour
        {
            if(m_colourIndex != -1 && m_colourIndex != a_dropdownIndex)
            {
                m_ignoreCallback = true;
                m_dropdowns[m_colourIndex].value = 0;
				m_ignoreCallback = false;
            }

            if(m_darknessIndex == a_dropdownIndex)
            {
                m_darknessIndex = -1;
            }
            m_colourIndex = a_dropdownIndex;
        }
        else if (a_dropdownValue == 2) // Darkness
        {
			if (m_darknessIndex != -1 && m_darknessIndex != a_dropdownIndex)
			{
				m_ignoreCallback = true;
				m_dropdowns[m_darknessIndex].value = 0;
				m_ignoreCallback = false;
			}

			if (m_colourIndex == a_dropdownIndex)
            {
                m_colourIndex = -1;
			}
            m_darknessIndex = a_dropdownIndex;
        }
        m_colourLegend.DisplayValuesChanged(m_colourIndex);
        m_darknessLegend.DisplayValuesChanged(m_darknessIndex);

		DataLoader.Instance.UpdateParticleVisualizationSettingsData(m_colourIndex, m_darknessIndex);
    }
}
