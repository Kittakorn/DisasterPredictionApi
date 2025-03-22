using System.Text.Json.Serialization;

namespace DisasterPrediction.Api.Models;

public class WeatherData
{
    [JsonPropertyName("main")]
    public Main Main { get; set; }

    [JsonPropertyName("wind")]
    public Wind Wind { get; set; }

    [JsonPropertyName("rain")]
    public Rain Rain { get; set; } = new Rain();
}

public class Main
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("sea_level")]
    public int SeaLevel { get; set; }

    [JsonPropertyName("grnd_level")]
    public int GrndLevel { get; set; }
}

public class Rain
{
    [JsonPropertyName("1h")]
    public double RainFall { get; set; } = 0;
}

public class Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Deg { get; set; }

    [JsonPropertyName("gust")]
    public double Gust { get; set; }
}