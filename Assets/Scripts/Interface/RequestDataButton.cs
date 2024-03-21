using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestDataButton : MonoBehaviour
{
    [SerializeField] private CustomButton m_requestButton; // Reference to the button in the Inspector
    [SerializeField] private CustomInputField m_startDateInput, m_endDateInput; // References to the input fields in the Inspector

    private string m_startDate, m_endDate; // The start and end dates of the data to be requested

    private void Awake()
    {
        // Add an onClick listener to the button
        m_requestButton.onClick.AddListener(SendRequest);
    }

    private void SendRequest()
    {
        // Set the start and end dates to the current date
        m_startDate = m_startDateInput.text;
        m_endDate = m_endDateInput.text;

        //TODO: Send a request to the server containing the particles' data.
    }
}
