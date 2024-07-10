using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FilterRange : MonoBehaviour
{
	[SerializeField] TMP_InputField m_minInput;
	[SerializeField] TMP_InputField m_maxInput;
	[SerializeField] Slider m_minSlider;
	[SerializeField] Slider m_maxSlider;
	[SerializeField] RectTransform m_sliderFill;
	[SerializeField] TextMeshProUGUI m_unit;

	float m_availableRangeMin;
	float m_availableRangeMax;
	bool m_ignoreCallbacks;
	float m_selectedRangeMin;
	float m_selectedRangeMax;
	float m_unitMultiplier;

	public Action m_onValueChangedCallback;
	public float SelectedRangeMin => m_selectedRangeMin / m_unitMultiplier;
	public float SelectedRangeMax => m_selectedRangeMax / m_unitMultiplier;

	public void SetAvailableRange(float a_min, float a_max, string a_unit = "m", float a_unitMultiplier = 1f, bool m_setRangeToMinMax = false)
	{
		m_ignoreCallbacks = true;
		m_unit.text = a_unit;
		m_unitMultiplier = a_unitMultiplier;
		m_availableRangeMin = a_min * a_unitMultiplier;
		m_availableRangeMax = a_max * a_unitMultiplier;
		m_minSlider.minValue = m_availableRangeMin;
		m_minSlider.maxValue = m_availableRangeMax;
		m_maxSlider.minValue = m_availableRangeMin;
		m_maxSlider.maxValue = m_availableRangeMax;
		if (m_setRangeToMinMax)
		{
			m_selectedRangeMin = m_availableRangeMin;
			m_selectedRangeMax = m_availableRangeMax;
		}
		else
		{
			if (m_selectedRangeMin < m_availableRangeMin)
				m_selectedRangeMin = m_availableRangeMin;
			else if (m_selectedRangeMin >= m_availableRangeMax)
				m_selectedRangeMin = m_availableRangeMax - 1f;
			if (m_selectedRangeMax < m_availableRangeMin)
				m_selectedRangeMax = m_availableRangeMin + 1f;
			else if (m_selectedRangeMax >= m_availableRangeMax)
				m_selectedRangeMax = m_availableRangeMax;
		}
		UpdateRangeDisplay();
		m_onValueChangedCallback?.Invoke();
		m_ignoreCallbacks = false;
	}

	private void Start()
	{
		m_minInput.onEndEdit.AddListener(OnMinInputChanged);
		m_maxInput.onEndEdit.AddListener(OnMaxInputChanged);
		m_minSlider.onValueChanged.AddListener(OnMinSliderChanged);
		m_maxSlider.onValueChanged.AddListener(OnMaxSliderChanged);
	}

	void OnMinInputChanged(string a_newValue)
	{
		if (m_ignoreCallbacks)
			return;

		m_ignoreCallbacks = true;
		if(!float.TryParse(a_newValue, out m_selectedRangeMin))
			m_selectedRangeMin = m_availableRangeMin;
		if (m_selectedRangeMin < m_availableRangeMin)
			m_selectedRangeMin = m_availableRangeMin;
		else if (m_selectedRangeMin > m_selectedRangeMax)
			m_selectedRangeMin = m_selectedRangeMax - 1f;
		UpdateRangeDisplay();
		m_onValueChangedCallback?.Invoke();
		m_ignoreCallbacks = false;
	}

	void OnMaxInputChanged(string a_newValue)
	{
		if (m_ignoreCallbacks)
			return;

		m_ignoreCallbacks = true;
		if (!float.TryParse(a_newValue, out m_selectedRangeMax))
			m_selectedRangeMax = m_availableRangeMin;
		if (m_selectedRangeMax < m_availableRangeMin)
			m_selectedRangeMax = m_availableRangeMin;
		else if (m_selectedRangeMax < m_selectedRangeMin)
			m_selectedRangeMax = m_selectedRangeMin + 1f;
		UpdateRangeDisplay();
		m_onValueChangedCallback?.Invoke();
		m_ignoreCallbacks = false;
	}

	void OnMinSliderChanged(float a_newValue)
	{
		if (m_ignoreCallbacks)
			return;

		m_ignoreCallbacks = true;
		m_selectedRangeMin = a_newValue;
		if (m_selectedRangeMin < m_availableRangeMin)
			m_selectedRangeMin = m_availableRangeMin;
		else if (m_selectedRangeMin > m_selectedRangeMax)
			m_selectedRangeMin = m_selectedRangeMax - 1f;
		UpdateRangeDisplay();
		m_onValueChangedCallback?.Invoke();
		m_ignoreCallbacks = false;
	}

	void OnMaxSliderChanged(float a_newValue)
	{
		if (m_ignoreCallbacks)
			return;

		m_ignoreCallbacks = true;
		m_selectedRangeMax = a_newValue;
		if (m_selectedRangeMax < m_availableRangeMin)
			m_selectedRangeMax = m_availableRangeMin;
		else if (m_selectedRangeMax < m_selectedRangeMin)
			m_selectedRangeMax = m_selectedRangeMin + 1f;
		UpdateRangeDisplay();
		m_onValueChangedCallback?.Invoke();
		m_ignoreCallbacks = false;
	}

	void UpdateRangeDisplay()
	{
		m_minInput.text = m_selectedRangeMin.ToString("N2");
		m_minSlider.value = m_selectedRangeMin;
		m_maxInput.text = m_selectedRangeMax.ToString("N2");
		m_maxSlider.value = m_selectedRangeMax;
		m_sliderFill.anchorMin = new Vector2(m_minSlider.normalizedValue, 0f);
		m_sliderFill.anchorMax = new Vector2(m_maxSlider.normalizedValue, 1f);
	}
}
