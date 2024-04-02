using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class APISetupWindow : MonoBehaviour
{
    [SerializeField]
    CustomButton m_setupAPIButton, m_closeButton, m_openSetupButton;

    [SerializeField]
    CustomInputField m_apiKeyInputField;

    [SerializeField]
    GameObject m_setupAPIWindow, m_canvasOutliner, m_invalidURLWindow;

    private void Awake()
    {
        m_setupAPIButton.onClick.AddListener(SetupAPI);
        m_closeButton.onClick.AddListener(CloseSetupWindow);
    }
    
    private void SetupAPI()
    {
        //Set the text of the button to the domain of the API
        m_openSetupButton.GetComponentInChildren<TextMeshProUGUI>().text = m_apiKeyInputField.text;

        // Setup API here
        if(SaveApiEndpoint())
        {
            CloseSetupWindow();
        }
    }

    private void CloseSetupWindow()
    {
        m_canvasOutliner.SetActive(false);
        m_setupAPIWindow.SetActive(false);
    }

    private bool SaveApiEndpoint()
    {
        string apiURL = m_apiKeyInputField.text;

        if(IsValidUrl(apiURL))
        {
            // Save the API endpoint here
            PlayerPrefs.SetString("APIEndpoint", apiURL);
            return true;
        }
        else
        {
            Debug.LogError("Invalid URL");
            InvalidURL();
            return false;
        }
    }

    private bool IsValidUrl(string url)
    {
        // Try to create a Uri from the input string
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult))
        {
            // Check if the scheme is HTTP or HTTPS
            return uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
        }

        return false;
    }

    private void InvalidURL()
    {
        m_invalidURLWindow.SetActive(true);
        m_setupAPIWindow.SetActive(false);
    }
}
