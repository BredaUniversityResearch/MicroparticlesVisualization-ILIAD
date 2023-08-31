using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu : MonoBehaviour
{
    [SerializeField]
    CustomToggle m_collapseToggle;

    [SerializeField]
    GameObject m_content;

    private void Awake()
    {
        m_collapseToggle.onValueChanged.AddListener((b) => m_content.SetActive(b));
    }
}
