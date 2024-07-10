using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CategoryFilter : MonoBehaviour
{
	[SerializeField] GameObject m_entryPrefab;
	[SerializeField] Transform m_entryParent;

	List<CategoryFilterEntry> m_entries;

	Action m_OnFilterChanged;
	int m_currentFilter;

	public int CurrentFilter => m_currentFilter;

	public void SetFilters(List<string> a_categories, Action a_onFilterChanged)
	{
		m_OnFilterChanged = a_onFilterChanged;
		if (m_entries != null)
		{
			foreach (var v in m_entries)
			{
				GameObject.Destroy(v.gameObject);
			}
		}
		m_entries = new List<CategoryFilterEntry>(a_categories.Count);
		m_currentFilter = int.MaxValue;
		for (int i = 0; i < a_categories.Count; i++)
		{
			CategoryFilterEntry newEntry = GameObject.Instantiate(m_entryPrefab, m_entryParent).GetComponent<CategoryFilterEntry>();
			int index = i;
			newEntry.SetContent(a_categories[i], (b) => OnEntryToggleChanged(index, b));
			m_entries.Add(newEntry);
		}
	}

	void OnEntryToggleChanged(int a_index, bool a_value)
	{
		if (a_value)
			m_currentFilter = m_currentFilter | (1 << a_index);
		else
			m_currentFilter = m_currentFilter & ~(1 << a_index);
		m_OnFilterChanged?.Invoke();
	}
}

