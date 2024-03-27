using System;
using UnityEngine;
using Microsoft.Research.Science.Data;

public class NetCDFConverter : MonoBehaviour
{
    string path = Application.dataPath + "/Data/opendrift-silcam-particle-transport-norkyst.nc";

    void Start()
    {
        ReadData(path);
    }

    public void ReadData(string path)
    {
        // Open the NetCDF file
        try
        {
            DataSet ds = DataSet.Open(path);
            Debug.Log("NetCDF file opened successfully");

            // Get the variables 'lon', 'lat', 'z', 'particle_size_distribution', 'particle_classification'
            Variable lon = ds["lon"];
            Variable lat = ds["lat"];
            Variable z = ds["z"];
            Variable particleSizeDistribution = ds["particle_size_distribution"];
            Variable particleClassification = ds["particle_classification"];

            // Print the first 10 elements of each variable
            PrintData(lon);
            PrintData(lat);
            PrintData(z);
            PrintData(particleSizeDistribution);
            PrintData(particleClassification);
        }
        catch (Exception ex)
        {
            Debug.Log($"Error reading NetCDF file: {ex.Message}");
        }
    }

    private void PrintData(Variable v)
    {
        Debug.Log("Variable: " + v.Name);

        // If the variable has data, print the first 10 elements of each dimension
        if (v.GetData() is Array data && data.Rank == 2)
        {
            for (int i = 0; i < Math.Min(10, data.GetLength(0)); i++)
            {
                for (int j = 0; j < Math.Min(10, data.GetLength(1)); j++)
                {
                    Debug.Log(data.GetValue(i, j) + ", ");
                }
            }
        }
    }
}