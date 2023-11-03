using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_widget, m_createWidget, m_canvasOutliner, m_popOutWindow;

    [SerializeField]
    CustomButton m_createWidgetButton;

    // Start is called before the first frame update
    void Awake()
    {
        m_createWidgetButton.onClick.AddListener(() => { EnablePopOut(); });
    }

    void EnablePopOut()
    {
        m_popOutWindow.SetActive(true);
        m_canvasOutliner.SetActive(true);
        m_popOutWindow.GetComponent<WidgetCreationManager>().SetWidget(m_widget, m_createWidget);
    }

}
