using System;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class LoadParticlesOnStart : MonoBehaviour
{
    public TextAsset m_depthCSVFile;
    public TextAsset m_latitudeCSVFile;
    public TextAsset m_longitudeCSVFile;

    bool m_initialized;

    //float m_timePassed = 10f;

    public void Update()
    {
        //m_timePassed += Time.deltaTime;
        //if(m_timePassed > 5f)
        //{
        //    m_timePassed = 0f;
        //    DataLoader.Instance.LoadNCDFFile(Application.dataPath + "/Data/sintefset.nc", OnLoadComplete);
        //}
		if (m_initialized)
			return;
		m_initialized = true;
		DataLoader.Instance.LoadNCDFFile(Application.dataPath + "/Data/sintefset.nc", OnLoadComplete);
    }

    void OnLoadComplete(bool a_success)
    {
        Debug.Log("Loading result: " + a_success.ToString());
    }

    private bool TryParseCSV(TextAsset a_csvFile, ref int a_entriesPerParticle, out float[] result)
    {
        string[] lines = a_csvFile.text.Split('\n');

        string[] entries = lines[0].Split(',');
        if (a_entriesPerParticle < 0)
        {
            a_entriesPerParticle = entries.Length;
        }
        else if (a_entriesPerParticle != entries.Length)
        {
            Debug.LogError($"Inconsistent amount of particle data on line 0 in file \"{a_csvFile.name}\", parsing failed.");
            result = null;
            return false;
        }
        result = new float[lines.Length * entries.Length];

        int i = 0;
        while (true)
        {

            if (a_entriesPerParticle < 0)
            {
                a_entriesPerParticle = entries.Length;
            }
            else if (a_entriesPerParticle != entries.Length)
            {
                Debug.LogError($"Inconsistent amount of particle data on line {i} in file \"{a_csvFile.name}\", parsing failed.");
                result = null;
                return false;
            }

            for (int j = 0; j < entries.Length; j++)
            {
                if (float.TryParse(entries[j], out float value))
                {
                    result[i * a_entriesPerParticle + j] = value;
                }
                else
                {
                    Debug.LogError($"Error parsing CSV value into float at row {i}, column {j} in file \"{a_csvFile.name}\"");
                    result = null;
                    return false;
                }
            }

            i++;
            if (i == lines.Length || lines[i] == "")
            {
                break;
            }
            else
                entries = lines[i].Split(',');
        }
        return true;
    }

    private bool TryParseCSVVert(TextAsset a_csvFile, out int a_entriesPerParticle, out float[] result)
    {
        string[] lines = a_csvFile.text.Split('\n');
        a_entriesPerParticle = lines.Length;

        string[] entries = lines[0].Split(',');
        int numberParticles = entries.Length;
        //int numberParticles = 92;
        result = new float[lines.Length * numberParticles];

        int i = 0;
        while (true)
        {
            //if (numberParticles != entries.Length)
            //{
            //	Debug.LogError($"Inconsistent amount of particle data on line {i} in file \"{a_csvFile.name}\", parsing failed. The line has {entries.Length} entries per instead of {numberParticles}.");
            //	result = null;
            //	return false;
            //}

            for (int j = 0; j < numberParticles; j++)
            {
                if (float.TryParse(entries[j], out float value))
                {
                    result[i + a_entriesPerParticle * j] = value;
                }
                else
                {
                    Debug.LogError($"Error parsing CSV value into float at row {i}, column {j} in file \"{a_csvFile.name}\"");
                    result = null;
                    return false;
                }
            }

            i++;
            if (i == lines.Length || lines[i] == "")
            {
                break;
            }

            entries = lines[i].Split(',');
        }

        // In case the last line is blank, i will be the correct number of entries in the CSV file.
        a_entriesPerParticle = Math.Min(i, a_entriesPerParticle);

        return true;
    }

}
