using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeasurementsManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_measurementEntryPRFB, m_measurementTable;

    [SerializeField]
    CustomInputField m_startDateInput, m_endDateInput;

    List<GameObject> m_instancedEntries = new List<GameObject>();
    DateTime m_startDate, m_endDate;

    // Start is called before the first frame update
    private void Start()
    {
        m_startDateInput.onEndEdit.AddListener(OnDateEndEdit);
        m_endDateInput.onEndEdit.AddListener(OnDateEndEdit);

        m_endDate = DateTime.Now.Date;
        m_startDate = DateTime.Now.Date.AddDays(-5);

        UpdateTable(m_startDate, m_endDate);
    }

    private string GenerateURL(DateTime a_dateStart, DateTime a_dateEnd, string a_buoy)
    {
        // Fixed beginning of the URL
        string baseURL = "https://oceanlab.azure.sintef.no/d/";

        // Append the string determined by the value of "origin"
        string originString = string.Empty;
        if (a_buoy == "munkholmen")
        {
            originString = "Pd5SYHD7k/environmental-data-munkholmen?orgId=1&from=";
        }
        else if (a_buoy == "ingdalen")
        {
            originString = "aImXqWjnz/environmental-data-ingdalen?orgId=1&from=";
        }

        // Convert dateStart and dateEnd to UNIX timestamps
        string dateStartUnix = ((DateTimeOffset)a_dateStart).ToUnixTimeSeconds().ToString().PadRight(13, '0');
        string dateEndUnix = ((DateTimeOffset)a_dateEnd).ToUnixTimeSeconds().ToString().PadRight(13, '0');

        // Construct the final URL
        string finalURL = $"{baseURL}{originString}{dateStartUnix}&to={dateEndUnix}";

        return finalURL;
    }

    public void UpdateTable(DateTime a_startDate, DateTime a_endDate)
    {
        int nextEntryIndex = 0;

        while (a_startDate < a_endDate)
        {
            DateTime rangeStart = a_startDate;
            DateTime rangeEnd = a_startDate.AddHours(12);

            if (rangeEnd > a_endDate)
            {
                rangeEnd = a_endDate;
            }

            // Generate URLs for both "munkholmen" and "ingdalen" origins!
            string urlMunkholmen = GenerateURL(rangeStart, rangeEnd, "munkholmen");
            string urlIngdalen = GenerateURL(rangeStart, rangeEnd, "ingdalen");

            UpdateEntry(rangeStart, urlMunkholmen, (nextEntryIndex + 1), nextEntryIndex, "munkholmen", "Buoy 1 - Munkholmen");
            nextEntryIndex++;

            UpdateEntry(rangeStart, urlIngdalen, (nextEntryIndex + 1), nextEntryIndex, "ingdalen", "Buoy 2 - Ingdalen");
            nextEntryIndex++;

            // Shift startDate by 12 hours for the next iteration
            a_startDate = a_startDate.AddHours(12);
        }

        // Remove any obsolete entries (if there are more existing entries than needed)
        for (int j = nextEntryIndex; j < m_instancedEntries.Count; j++)
        {
            Destroy(m_instancedEntries[j]);
        }

        m_instancedEntries.RemoveRange(nextEntryIndex, m_instancedEntries.Count - nextEntryIndex);
    }

    private void UpdateEntry(DateTime a_rangeStart,string a_url, int a_entryCode, int a_existingEntryIndex, string a_buoy, string a_buoyText)
    {
        GameObject tempInstance;

        // Check if there's an existing entry at the current index
        if (a_existingEntryIndex < m_instancedEntries.Count)
        {
            // Update the existing entry
            m_instancedEntries[a_existingEntryIndex].GetComponent<TableEntry>().SetContent("Entry " + a_entryCode, a_buoy, a_buoyText, a_rangeStart.ToString(), a_url);
        }
        else
        {
            // Instantiate a new entry
            tempInstance = Instantiate(m_measurementEntryPRFB, m_measurementTable.transform);
            tempInstance.GetComponent<TableEntry>().SetContent("Entry " + a_entryCode, a_buoy, a_buoyText, a_rangeStart.ToString("dd/MM/yyyy HH:mm"), a_url);
            m_instancedEntries.Add(tempInstance);
        }
    }

    private bool TryParseDate(string a_dateStr, out DateTime a_date)
    {
        return DateTime.TryParseExact(a_dateStr, new[] { "dd/MM/yyyy", "dd-MM-yyyy" }, null, System.Globalization.DateTimeStyles.None, out a_date);
    }

    private void OnDateEndEdit(string a_newDate)
    {
        DateTime startDate;
        DateTime endDate;

        if (TryParseDate(m_startDateInput.text, out startDate) && TryParseDate(m_endDateInput.text, out endDate))
        {
            // Both input fields have valid dates, call UpdateTable
            UpdateTable(startDate, endDate);
        }
    }
}
