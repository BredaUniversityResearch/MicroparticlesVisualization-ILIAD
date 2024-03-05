using System;
using UnityEngine;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;

public class NetCDFConverter : MonoBehaviour
{
    string path = @"path_to_your_netCDF_file.nc";


    void Start()
    {
        ReadNetCDFFile(path);
    }

    public void ReadNetCDFFile(string path)
    {
        try
        {
            // Open the NetCDF file
            DataSet ds = DataSet.Open(path);

            // Access a specific variable
            Variable var = ds.Variables["YourVariableName"];

            // Read data from the variable
            Array data = var.GetData();
            // Process data...

            Debug.Log("Data read successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading NetCDF file: {ex.Message}");
        }
    }
}