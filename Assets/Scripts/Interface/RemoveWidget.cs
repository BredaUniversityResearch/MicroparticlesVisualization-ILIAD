using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveWidget : MonoBehaviour
{
    [SerializeField]
    CustomButton m_closeButton;

    [SerializeField]
    GameObject m_widget, m_createWidget;
    // Start is called before the first frame update
    void Awake()
    {
        m_closeButton.onClick.AddListener(() => { CloseWidget(); });
    }

    void CloseWidget()
    {
        m_widget.SetActive(false);
        m_createWidget.SetActive(true);
    }
}
