using System;
using System.Collections.Generic;

[Serializable]
public class Coord
{
    public double lon;
    public double lat;
}

[Serializable]
public class Weather
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

[Serializable]
public class Main
{
    public double temp;
    public double feels_like;
    public double temp_min;
    public double temp_max;
    public int pressure;
    public int sea_level;
    public int grnd_level;
    public int humidity;
}

[Serializable]
public class Wind
{
    public double speed;
    public int deg;
    public double gust;
}

[Serializable]
public class Rain
{
    public double _1h;
}

[Serializable]
public class Clouds
{
    public int all;
}

[Serializable]
public class Sys
{
    public string pod;
}

[Serializable]
public class DaysList
{
    public long dt;
    public Main main;
    public List<Weather> weather;
    public Clouds clouds;
    public Wind wind;
    public int visibility;
    public double pop;
    public Rain rain;
    public Sys sys;
    public string dt_txt;
}

[Serializable]
public class City
{
    public int id;
    public string name;
    public Coord coord;
    public string country;
    public int population;
    public int timezone;
    public long sunrise;
    public long sunset;
}

[Serializable]
public class WeatherData
{
    public string cod;
    public int message;
    public int cnt;
    public List<DaysList> list;
    public City city;
}
