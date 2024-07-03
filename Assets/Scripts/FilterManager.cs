using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;

public class FilterManager : MonoBehaviour
{
	static FilterManager m_instance;
	public static FilterManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = FindObjectOfType<FilterManager>();
			}
			if (m_instance == null)
			{
				Debug.LogError("No FilterManager found in the scene.");
			}
			return m_instance;
		}
	}

	[SerializeField] FilterRange m_depthFilter;
	[SerializeField] FilterRange m_sizeFilter;
	[SerializeField] CategoryFilter m_typeFilter;

	List<string> m_types;

	private void Awake()
	{
		if(m_instance != null && m_instance != this)
		{
			Debug.LogError("Multiple FilterManagers found in the scene.");
		}
		m_instance = this;
		m_depthFilter.m_onValueChangedCallback = OnDepthSizeFilterChanged;
		m_sizeFilter.m_onValueChangedCallback = OnDepthSizeFilterChanged;
	}

	public void SetFilterRanges(float a_depthMin, float a_depthMax, float a_sizeMin, float a_sizeMax, List<string> a_types)
	{
		m_depthFilter.SetAvailableRange(a_depthMin, a_depthMax, true);
		m_sizeFilter.SetAvailableRange(a_sizeMin, a_sizeMax, true);
		m_typeFilter.SetFilters(a_types, OnDepthSizeFilterChanged);
		m_types = a_types;
		OnDepthSizeFilterChanged();
	}

	void OnDepthSizeFilterChanged()
	{
		// Get the active world and the EntityManager
		var world = World.DefaultGameObjectInjectionWorld;
		var entityManager = world.EntityManager;

		// Create a query to get the Entity with the ParticleVisualizationSettingsData component
		EntityQuery query = entityManager.CreateEntityQuery(typeof(ParticleVisualizationSettingsData));
		if (!query.HasSingleton<ParticleVisualizationSettingsData>())
		{
			Debug.LogError("No 'ParticleVisualizationSettingsData' component found. Setting particle visualization settings data failed.");
			return;
		}
		Entity settingsEntity = query.GetSingletonEntity();

		if (entityManager.Exists(settingsEntity))
		{
			var settingsData = entityManager.GetComponentData<ParticleVisualizationSettingsData>(settingsEntity);

			settingsData.m_sizeDepthFilter = new Unity.Mathematics.float4(
				m_sizeFilter.SelectedRangeMin, m_sizeFilter.SelectedRangeMax,
				m_depthFilter.SelectedRangeMin, m_depthFilter.SelectedRangeMax);
			settingsData.m_typeFilter = m_typeFilter.CurrentFilter;
			settingsData.m_numberTypes = m_types == null ? 0 : m_types.Count;

			entityManager.SetComponentData(settingsEntity, settingsData);
		}
	}
}

