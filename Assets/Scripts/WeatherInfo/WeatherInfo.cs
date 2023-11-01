using System;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class WeatherInfo : MonoBehaviour
{
    HttpClient httpClient = new HttpClient();
    string m_lon = "10.374";
    string m_lat = "63.458";
    string m_appid = "d25a7e67f8723dff49c13d45fed0dddf";
    string m_apiUrl = "https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&units=metric&cnt=6&appid={API_key}";
    
    [SerializeField]
    SpriteDictionary m_spriteDictionary;

    [SerializeField]
    CustomToggle m_homePageToggle;

    [SerializeField]
    TextMeshProUGUI m_locationText, m_temperatureText, m_weatherText, m_precipitationText, m_windText, m_humidityText;

    [SerializeField]
    Image m_weatherImage;

    [SerializeField]
    List<SubWidget> m_subWidgetList;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CallWeatherApi());
        m_homePageToggle.onValueChanged.AddListener((isOn) => {
            if (isOn)
            {
                StartCoroutine(CallWeatherApi());
            }
        });
    }
    
    string ComposeUrl()
    {
        string finalApiUrl = m_apiUrl.Replace("{lat}", m_lat).Replace("{lon}", m_lon).Replace("{API_key}", m_appid);
        Debug.Log("The completed url is: " + finalApiUrl);
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
            Debug.Log(jsonResponse);
            WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonResponse);

            UpdateWidget(weatherData);
        }            
    }

    void UpdateWidget(WeatherData a_weatherData)
    {
        m_locationText.text = a_weatherData.city.name;
        m_temperatureText.text = a_weatherData.list[0].main.temp.ToString() + "°C";
        m_weatherText.text = a_weatherData.list[0].weather[0].main;
        m_precipitationText.text = a_weatherData.list[0].rain._1h.ToString() + " mm";
        m_windText.text = a_weatherData.list[0].wind.speed.ToString() + " km/h";
        m_humidityText.text = a_weatherData.list[0].main.humidity.ToString() + "%";
        m_weatherImage.sprite = m_spriteDictionary.GetSprite(a_weatherData.list[0].weather[0].main);

        for (int i = 1; i < 5; i++)
        {
            UpdateSubWidget(a_weatherData, i, m_subWidgetList[i-1]);
        }
    }

    void UpdateSubWidget(WeatherData a_weatherData, int a_iteration, SubWidget a_subWidget)
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

}
