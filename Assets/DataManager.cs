using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset depthCSVFile;
    public TextAsset latitudeCSVFile;
    public TextAsset longitudeCSVFile;

    private float[,] depthMatrix;
    private float[,] latitudeMatrix;
    private float[,] longitudeMatrix;

    private void Start()
    {
        // Load CSV data from TextAssets
        depthMatrix = LoadCSV(depthCSVFile);
        latitudeMatrix = LoadCSV(latitudeCSVFile);
        longitudeMatrix = LoadCSV(longitudeCSVFile);

        // Now you have your data in depthMatrix, latitudeMatrix, and longitudeMatrix
    }

    // Function to parse CSV data into a matrix
    private float[,] LoadCSV(TextAsset csvFile)
    {
        string[] lines = csvFile.text.Split('\n');
        int numRows = lines.Length;
        int numCols = lines[0].Split(',').Length;
        float[,] matrix = new float[numRows, numCols];

        for (int i = 0; i < numRows; i++)
        {
            string[] values = lines[i].Split(',');
            for (int j = 0; j < numCols; j++)
            {
                if (float.TryParse(values[j], out float value))
                {
                    matrix[i, j] = value;
                }
                else
                {
                    // Handle parsing error if needed
                    Debug.LogError("Error parsing CSV at row " + (i + 1) + ", column " + (j + 1));
                }
            }
        }

        return matrix;
    }

}
