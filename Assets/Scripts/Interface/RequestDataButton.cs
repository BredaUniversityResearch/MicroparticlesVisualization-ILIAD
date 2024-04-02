using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestDataButton : MonoBehaviour
{
    // Reference to the button in the Inspector
    [SerializeField] 
    private CustomButton m_requestButton, m_setupAPI;

    // References to the input fields in the Inspector
    [SerializeField] 
    private CustomInputField m_startDateInput, m_endDateInput;
    
    [SerializeField]
    private GameObject m_canvasOutliner, m_setupAPIWindow;

    private string m_startDate, m_endDate; // The start and end dates of the data to be requested

    private void Awake()
    {
        // Add an onClick listener to the button
        m_requestButton.onClick.AddListener(SendRequest);
        m_setupAPI.onClick.AddListener(SetupAPI);
    }

    private void SendRequest()
    {
        // Set the start and end dates to the current date
        m_startDate = m_startDateInput.text;
        m_endDate = m_endDateInput.text;

        //TODO: Send a request to the server containing the particles' data.
    }

    private void SetupAPI()
    {
        m_canvasOutliner.SetActive(true);
        m_setupAPIWindow.SetActive(true);
    }
}
