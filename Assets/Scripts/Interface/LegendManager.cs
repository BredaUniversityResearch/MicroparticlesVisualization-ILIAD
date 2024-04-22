using ColourPalette;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;
using System;

public class LegendManager : MonoBehaviour
{
    [SerializeField]
    DropdownMenu m_typeColourDropdown, m_sizeColourDropdown, m_depthColourDropdown;

    [SerializeField]
    DropdownMenu m_typeDarknessDropdown, m_sizeDarknessDropdown, m_depthDarknessDropdown;

    [SerializeField]
    Texture2D m_rainbowLine;

    private void Awake()
    {
        SetLegendColours(m_typeColourDropdown);
        SetLegendColours(m_sizeColourDropdown);
        SetLegendColours(m_depthColourDropdown);

        SetLegendDarkness(m_typeDarknessDropdown);
        SetLegendDarkness(m_sizeDarknessDropdown);
        SetLegendDarkness(m_depthDarknessDropdown);
    }

    private void SetLegendColours(DropdownMenu a_colourDropdown)
    {
        // Get the "Content" child of the dropdown
        Transform content = a_colourDropdown.transform.GetChild(1);

        // Get the number of LegendEntries
        int numEntries = content.childCount;

        // Iterate over each LegendEntry
        for (int i = 0; i < numEntries; i++)
        {
            // Get the LegendEntry
            Transform legendEntry = content.GetChild(i);

            // Get the CustomImage component
            CustomImage customImage = legendEntry.GetChild(0).GetComponent<CustomImage>();

            // Calculate the position in the rainbow line
            float position = CalculatePosition(i, numEntries);

            // Get the color from the rainbow line at the calculated position
            Color color = m_rainbowLine.GetPixelBilinear(position, 0.5f);

            // Set the color of the CustomImage
            customImage.color = color;
        }
    }

    // Define a function to calculate the position in the rainbow line
    float CalculatePosition(int a_index, int a_numEntries)
    {
        if (a_index == 0)
            return 1f;
        else if (a_index == a_numEntries - 1)
            return 0f;
        else
            return 1f - ((1f / a_numEntries) * (a_index + 1));
    }


    private void SetLegendDarkness(DropdownMenu a_darknessDropdown)
    {
        // Get the "Content" child of the dropdown
        Transform content = a_darknessDropdown.transform.GetChild(1);

        // Get the number of LegendEntries
        int numEntries = content.childCount;

        // Iterate over each LegendEntry
        for (int i = 0; i < numEntries; i++)
        {
            // Get the LegendEntry
            Transform legendEntry = content.GetChild(i);

            // Get the CustomImage component
            CustomImage customImage = legendEntry.GetChild(0).GetComponent<CustomImage>();

            float position = CalculatePosition(i, numEntries);

            //TODO: Set the darkness of the CustomImage based on the position in the rainbow line
            // Just change 1f to i
            Color color = m_rainbowLine.GetPixelBilinear(1f, 0.5f);

            color = CalculateDarkness(i, numEntries, color);

            // Set the color of the CustomImage
            customImage.color = color;            
        }
    }

    private Color CalculateDarkness(int a_index, int a_numEntries, Color a_color)
    {
        // Calculate the position using the provided method
        float position = CalculatePosition(a_index, a_numEntries);

        // Multiply the color by the darkness intensity
        Color darkColor = a_color * position;

        if(position == 0f)
        {
            darkColor = a_color * 0.1f;
        }

        darkColor.a = 1f;

        return darkColor;
    }
}
