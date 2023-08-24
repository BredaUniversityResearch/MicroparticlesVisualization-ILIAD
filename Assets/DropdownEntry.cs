using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownEntry : MonoBehaviour
{
    [SerializeField]
    GameObject m_content, toggleEntry_prefab;

    public void AddEntry(string text)
    {
        GameObject obj = Instantiate(toggleEntry_prefab, m_content.transform);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
