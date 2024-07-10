using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CategoryFilterEntry : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI m_name;
	[SerializeField] Toggle m_toggle;

	bool m_ignoreCallback;
	Action<bool> m_onChangeCallback;

	public bool Toggled
	{
		get { return m_toggle.isOn; }
		set {
			m_ignoreCallback = true;
			m_toggle.isOn = value;
			m_ignoreCallback = false;
		}
	}

	public void SetContent(string a_name, Action<bool> a_onChangeCallback)
	{
		m_name.text = a_name;
		m_toggle.onValueChanged.RemoveAllListeners();
		m_onChangeCallback = a_onChangeCallback;
		m_toggle.onValueChanged.AddListener(OnToggleChanged);
	}

	void OnToggleChanged(bool a_value)
	{
		if (m_ignoreCallback)
			return;

		m_onChangeCallback?.Invoke(a_value);
	}

}

