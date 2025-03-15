using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Services;

public class OpenWeatherService : IOpenWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenWeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");
    }

    public async Task<WeatherData> GetWeatherDataAsync(decimal latitude, decimal longitude)
    {
        var url = $"weather?lat={latitude}&lon={longitude}&appid={_apiKey}";
        return await _httpClient.GetFromJsonAsync<WeatherData>(url);
    }
}
