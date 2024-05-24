using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class WeatherInfo : MonoBehaviour
{
    [SerializeField]
    string m_lon = "10.374";

    [SerializeField]
    string m_lat = "63.458";

    string m_appid_path = "Assets/Secrets/OpenWeatherAPIKey.txt";
    string m_appid;
    string m_apiEndpoint = "https://api.openweathermap.org/data/2.5/forecast";

    [SerializeField]
    SpriteDictionary m_spriteDictionary;

    [SerializeField]
    CustomToggle m_homePageToggle;

    [SerializeField]
    TextMeshProUGUI m_locationText, m_dateText, m_temperatureText, m_weatherText, m_precipitationText, m_windText, m_humidityText;

    [SerializeField]
    Image m_weatherImage;

    [SerializeField]
    List<SubWidget> m_subWidgetList;

    // Start is called before the first frame update
    void Start()
    {
        m_appid = File.ReadAllText(m_appid_path);

        StartCoroutine(CallWeatherApi());
        m_homePageToggle.onValueChanged.AddListener((isOn) => {
            if (isOn)
            {
                StartCoroutine(CallWeatherApi());
            }
        });
    }

    public void WidgetCreation(string a_lon, string a_lat)
    {
        m_lon = a_lon;
        m_lat = a_lat;
        StartCoroutine(CallWeatherApi());
    }
    
    string ComposeUrl()
    {
        string finalApiUrl = $"{m_apiEndpoint}?lat={m_lat}&lon={m_lon}&units=metric&cnt=6&appid={m_appid}";
        //Debug.Log("The completed url is: " + finalApiUrl);
        return finalApiUrl;
    }

    IEnumerator CallWeatherApi()
    {
        string finalApiUrl = ComposeUrl();

        UnityWebRequest request = UnityWebRequest.Get(finalApiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Request was successful
            string jsonResponse = request.downloadHandler.text;
            //Debug.Log(jsonResponse);

            Root weatherData = JsonConvert.DeserializeObject<Root>(jsonResponse);

            UpdateWidget(weatherData);
        }            
    }

    void UpdateWidget(Root a_weatherData)
    {
        m_locationText.text = a_weatherData.city.name;
        m_dateText.text = ExtractDatefromDateTimeString(a_weatherData.list[0].dt_txt);
        m_temperatureText.text = a_weatherData.list[0].main.temp.ToString() + "°C";
        m_weatherText.text = a_weatherData.list[0].weather[0].main;
        if(a_weatherData.list[0].rain != null)
        {
            m_precipitationText.text = a_weatherData.list[0].rain["3h"].ToString() + "mm/3hrs";

        }

        m_windText.text = a_weatherData.list[0].wind.speed.ToString() + " km/h";
        m_humidityText.text = a_weatherData.list[0].main.humidity.ToString() + "%";
        m_weatherImage.sprite = m_spriteDictionary.GetSprite(a_weatherData.list[0].weather[0].main);

        for (int i = 1; i < 5; i++)
        {
            UpdateSubWidget(a_weatherData, i, m_subWidgetList[i-1]);
        }
    }

    void UpdateSubWidget(Root a_weatherData, int a_iteration, SubWidget a_subWidget)
    {
        a_subWidget.m_temperatureText.text = a_weatherData.list[a_iteration].main.temp.ToString() + "°C";
        a_subWidget.m_timeText.text = ExtractTimeFromDateTimeString(a_weatherData.list[a_iteration].dt_txt);
        a_subWidget.m_weatherImage.sprite = m_spriteDictionary.GetSprite(a_weatherData.list[a_iteration].weather[0].main);
    }
    
    public string ExtractTimeFromDateTimeString(string a_dateTimeString)
    {
        // Parse the input string as a DateTime
        if (DateTime.TryParse(a_dateTimeString, out DateTime dateTime))
        {
            // Format the DateTime to "HH:mm" using a custom format string
            string formattedTime = dateTime.ToString("HH:mm");
            return formattedTime;
        }
        else
        {
            // Handle parsing error, e.g., invalid format
            return "Invalid Format";
        }
    }

    public string ExtractDatefromDateTimeString(string a_dateTimeString)
    {
        // Parse the input string as a DateTime
        if (DateTime.TryParse(a_dateTimeString, out DateTime dateTime))
        {
            // Format the DateTime to "HH:mm" using a custom format string
            string formattedTime = dateTime.ToString("dd MMMM");
            return formattedTime;
        }
        else
        {
            // Handle parsing error, e.g., invalid format
            return "Invalid Format";
        }
    }

}
