using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Services;

public class UsgsService : IUsgsService
{
    private readonly HttpClient _httpClient;
    private const int MaxRadiusKm = 50;

    public UsgsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EarthquakeData> GetEarthquakeDataAsync(decimal latitude, decimal longitude)
    {
        var updateAfter = DateTime.Now.ToUniversalTime()
            .AddMinutes(-15)
            .ToString("yyyy-MM-ddTHH:mm:ss");

        var url = $"query?format=geojson&latitude={latitude}&longitude={longitude}&updatedafter={updateAfter}&maxradiuskm={MaxRadiusKm}";
        return await _httpClient.GetFromJsonAsync<EarthquakeData>(url);
    }
}