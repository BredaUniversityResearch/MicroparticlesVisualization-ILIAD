using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalButton : MonoBehaviour
{
    [SerializeField]
    CustomButton m_externalButton;

    [SerializeField]
    string m_url;

    private void Awake()
    {
        m_externalButton.onClick.AddListener(OpenExternalURL);
    }

    private void OpenExternalURL()
    {
        Application.OpenURL(m_url);
    }
}
