using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideUntilLoaded : MonoBehaviour
{
	[SerializeField] List<GameObject> m_objectsActiveAfterLoad;

	private void Awake()
	{
		DataLoader.Instance.m_onLoadStartEvent += OnLoadStart;
		DataLoader.Instance.m_onLoadEndEvent += OnLoadEnd;
		SetObjectsActive(false);
	}

	void OnLoadStart()
	{
		SetObjectsActive(false);
	}

	void OnLoadEnd(bool a_success)
	{ 
		if(a_success)
		{
			SetObjectsActive(true);
		}
	}

	void SetObjectsActive(bool a_active)
	{
		foreach (GameObject obj in m_objectsActiveAfterLoad)
			obj.SetActive(a_active);
	}
}

