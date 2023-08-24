using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapseLeftBar : MonoBehaviour
{
    [SerializeField]
    GameObject m_leftBar, m_mainScreen;
    
    [SerializeField]
    List<GameObject> m_itemsToDeactivate;

    [SerializeField]
    CustomButton m_collapseButton, m_expandButton;

    private void Awake()
    {
        //Have it call the function to expand/collapse.
        m_collapseButton.onClick.AddListener(Collapse);
        m_expandButton.onClick.AddListener(Expand);
    }

    void Collapse()
    {
        RectTransform leftBarTransform = m_leftBar.GetComponent<RectTransform>();
        RectTransform mainScreenTransform = m_mainScreen.GetComponent<RectTransform>();

        leftBarTransform.sizeDelta = new Vector2(100, 1080);
        mainScreenTransform.offsetMin = new Vector3(100, 0, 0);
        //mainScreenTransform.sizeDelta = new Vector2(100, 0);

        foreach (GameObject item in m_itemsToDeactivate)
        {
            item.SetActive(false);
        }

        m_collapseButton.gameObject.SetActive(false);
        m_expandButton.gameObject.SetActive(true);
    }

    void Expand()
    {
        RectTransform leftBarTransform = m_leftBar.GetComponent<RectTransform>();
        RectTransform mainScreenTransform = m_mainScreen.GetComponent<RectTransform>();

        leftBarTransform.sizeDelta = new Vector2(300, 1080);
        mainScreenTransform.offsetMin = new Vector3(300, 0, 0);

        foreach (GameObject item in m_itemsToDeactivate)
        {
            item.SetActive(true);
        }

        m_collapseButton.gameObject.SetActive(true);
        m_expandButton.gameObject.SetActive(false);
    }
}
