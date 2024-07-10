using ColourPalette;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LegendEntry : MonoBehaviour
{
	[SerializeField] Image m_colourImage;
	[SerializeField] TextMeshProUGUI m_text;

	public void Setvalues(Color a_colour, string a_text)
	{
		gameObject.SetActive(true);
		m_colourImage.color = a_colour;
		m_text.text = a_text;
	}
}

