using ColourPalette;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crosstales.FB;

public class LoadFileButton : MonoBehaviour
{
    [SerializeField] Button m_button;

    bool m_selectorOpen;

    private void Awake()
    {
        m_button.onClick.AddListener(OnButtonPress);
    }

    void OnButtonPress()
	{
        if (m_selectorOpen)
            return;
        m_selectorOpen = true;
        FileBrowser.Instance.OnOpenFilesComplete += OnOpenFilesComplete;
        FileBrowser.Instance.OpenSingleFileAsync("nc");
    }

    private void OnOpenFilesComplete(bool a_selected, string a_singlefile, string[] a_multiFiles)
    {
        m_selectorOpen = false;
        if (FileBrowser.Instance != null)
            FileBrowser.Instance.OnOpenFilesComplete -= OnOpenFilesComplete;
        if(a_selected)
		{
            DataLoader.Instance.LoadNCDFFile(a_singlefile.Replace('\\', '/'), null);
        }
    }
}
