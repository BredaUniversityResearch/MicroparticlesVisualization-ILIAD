using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
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

	private void Awake()
	{
		if(m_instance != null && m_instance != this)
		{
			Debug.LogError("Multiple FilterManagers found in the scene.");
		}
		m_instance = this;
		m_depthFilter.m_onValueChangedCallback = OnDepthSizeFilterChanged;
		m_sizeFilter.m_onValueChangedCallback = OnDepthSizeFilterChanged;
		//TODO: deal with particle type toggles
	}

	public void SetFilterRanges(float a_depthMin, float a_depthMax, float a_sizeMin, float a_sizeMax, string[] a_types)
	{
		m_depthFilter.SetAvailableRange(a_depthMin, a_depthMax, true);
		m_sizeFilter.SetAvailableRange(a_sizeMin, a_sizeMax, true);
		//TODO: deal with particle type toggles
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

			entityManager.SetComponentData(settingsEntity, settingsData);
		}
	}
}

