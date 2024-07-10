using ColourPalette;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class LegendDisplay : MonoBehaviour
{
	[SerializeField] GameObject m_entryPrefab;
	[SerializeField] Transform m_entryParent;
	[SerializeField] Texture2D m_displayGradient;

	List<LegendEntry> m_entries = new List<LegendEntry>();
	bool m_initialised;
	int m_dataIndex;

	private void Start()
	{
		if (!m_initialised)
			gameObject.SetActive(false);
	}

	void Initialise()
	{
		m_initialised = true;
		FilterManager.Instance.m_onFiltersChangedCallback += OnFilterschanged;
	}

	void OnFilterschanged()
	{
		if (m_dataIndex >= 0)
		{
			gameObject.SetActive(true);
			switch (m_dataIndex)
			{
				case 0:
					SetToCategory();
					break;
				case 1:
					SetToRange(FilterManager.Instance.SizeFilter, " mm diameter", 1000f);
					break;
				case 2:
					SetToRange(FilterManager.Instance.DepthFilter, " m depth", -1f);
					break;
			}
		}
		else
			gameObject.SetActive(false);
	}

	public void DisplayValuesChanged(int a_dataIndex)
	{
		m_dataIndex = a_dataIndex;
		if (!m_initialised)
			Initialise();
		OnFilterschanged();
	}

	void SetToCategory()
	{
		List<string> types = FilterManager.Instance.Types;
		if (types.Count == 1)
		{
			if (m_entries.Count < 1)
			{
				LegendEntry entry = Instantiate(m_entryPrefab, m_entryParent).GetComponent<LegendEntry>();
				m_entries.Add(entry);
			}
			m_entries[0].Setvalues(m_displayGradient.GetPixelBilinear(0f, 0.5f), types[0]);
		}
		else
		{
			for (int i = 0; i < types.Count; i++)
			{
				if (m_entries.Count <= i)
				{
					LegendEntry entry = Instantiate(m_entryPrefab, m_entryParent).GetComponent<LegendEntry>();
					m_entries.Add(entry);
				}
				m_entries[i].Setvalues(m_displayGradient.GetPixelBilinear(i / ((float)types.Count - 1f), 0.5f), types[i]);
			}
		}
		for(int i = types.Count; i < m_entries.Count; i++)
		{
			m_entries[i].gameObject.SetActive(false);
		}
	}

	void SetToRange(Vector2 a_range, string a_unitAndParam, float a_scale)
	{
		for (int i = 0; i < 5; i++)
		{
			if (m_entries.Count <= i)
			{
				LegendEntry entry = Instantiate(m_entryPrefab, m_entryParent).GetComponent<LegendEntry>();
				m_entries.Add(entry);
			}
			float t = i / 4f;
			m_entries[i].Setvalues(m_displayGradient.GetPixelBilinear(t, 0.5f), ((a_range.x + t * (a_range.y - a_range.x)) * a_scale).ToString("N2") + a_unitAndParam);
		}
		for (int i = 5; i < m_entries.Count; i++)
		{
			m_entries[i].gameObject.SetActive(false);
		}
	}
}

