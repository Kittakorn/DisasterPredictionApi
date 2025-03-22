using DisasterPrediction.Api.Interfaces;
using DisasterPrediction.Api.Models;
using System.Text.Json;

namespace DisasterPrediction.Api.Services;

public class OpenWeatherService : IOpenWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<OpenWeatherService> _logger;

    public OpenWeatherService(HttpClient httpClient, ILogger<OpenWeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");
    }

    public async Task<WeatherData> GetWeatherDataAsync(string cityName, string countryCode = "TH")
    {
        var response = await _httpClient.GetAsync($"weather?q={cityName},countryCode={countryCode}&appid={_apiKey}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var weatherData = JsonSerializer.Deserialize<WeatherData>(content);
            return weatherData;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Regions {CityName} with no available data", cityName);
            return null;
        }

        throw new BadHttpRequestException($"Failed to get weather data {response.StatusCode} {response.Content}");
    }
}
