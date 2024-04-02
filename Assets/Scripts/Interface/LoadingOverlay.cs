using ColourPalette;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingOverlay : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
        DataLoader.Instance.m_onLoadStartEvent += OnDataLoadStart;
        DataLoader.Instance.m_onLoadEndEvent += OnDataLoadEnd;
    }

    void OnDataLoadStart()
    {
        gameObject.SetActive(true);
    }

    void OnDataLoadEnd(bool a_success)
    {
        gameObject.SetActive(false);
    }
}
