using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlertCreationManager : MonoBehaviour
{
    [SerializeField]
    CustomInputField m_inputName, m_inputRule;

    [SerializeField]
    TextMeshProUGUI m_variableDropdown;

    [SerializeField]
    CustomToggle m_isSimulatedToggle, m_isRealTimeToggle;

    [SerializeField]
    AlertEntryManager m_alertEntry;

    [SerializeField]
    Transform m_realTimeTable, m_simulatedTable;

    [SerializeField]
    CustomButton m_createAlertButton;

    [SerializeField]
    GameObject m_popOutWindow, m_canvasOutliner;

    private void Awake()
    {
        m_createAlertButton.onClick.AddListener(() => { m_alertEntry.AlertCreation(m_inputName, m_inputRule, m_variableDropdown, m_isRealTimeToggle, m_isSimulatedToggle, m_realTimeTable, m_simulatedTable); CleanUp(); m_canvasOutliner.SetActive(false); });
    }

    private void CleanUp()
    {
        m_inputName.text = "";
        m_inputRule.text = "";
        m_popOutWindow.SetActive(false);
    }
}
