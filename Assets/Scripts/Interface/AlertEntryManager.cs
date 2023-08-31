using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ColourPalette;

public class AlertEntryManager : MonoBehaviour
{
    [SerializeField]
    CustomButton m_removeAlertButton;

    [SerializeField]
    GameObject m_alert;

    [SerializeField]
    CustomText m_name, m_variable, m_alertRule;

    private void Awake()
    {
        m_removeAlertButton.onClick.AddListener(() => Destroy(m_alert));
    }

    public void AlertCreation(CustomInputField name, CustomInputField rule, TextMeshProUGUI variable, CustomToggle isRealTimeToggle, CustomToggle isSimulatedToggle, Transform realTimeTable, Transform simulatedTable)
    {
        m_name.text = name.text;
        m_variable.text = variable.text;
        m_alertRule.text = rule.text;

        if (isSimulatedToggle.isOn)
        {
            Instantiate(m_alert, simulatedTable);
        }

        if (isRealTimeToggle.isOn)
        {
            Instantiate(m_alert, realTimeTable);
        }

    }
}
