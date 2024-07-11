using System.Collections;
using System.Collections.Generic;
using System.Net;
//using UnityEditor;
using UnityEngine;
using TMPro;

public class FirstSetup : MonoBehaviour
{
    [SerializeField]
    private GameObject m_firstSetupPRFB, m_canvasOutliner;

    [SerializeField]
    private CustomButton m_setupApiButton;
    
    private const string PrefabInstantiatedKey = "PrefabInstantiated";
    private const string APIEndpoint = "APIEndpoint";

    private void Start()
    {
        if (!PlayerPrefs.HasKey(PrefabInstantiatedKey))
        {
            // Instantiate your prefab here
            InstantiatePrefab();

            // Set the flag to indicate that the prefab has been instantiated
            PlayerPrefs.SetInt(PrefabInstantiatedKey, 1);
        }

        if(PlayerPrefs.HasKey(APIEndpoint))
        {
            m_setupApiButton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerPrefs.GetString(APIEndpoint);
        }

        /*if(EditorApplication.isPlaying)
        {
            // Instantiate your prefab here
            InstantiatePrefab();
        }*/
    }

    private void InstantiatePrefab()
    {
        m_firstSetupPRFB.SetActive(true);
        m_canvasOutliner.SetActive(true);
    }
}
