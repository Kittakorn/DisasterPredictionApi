using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Interfaces;

public interface IUsgsService
{
    Task<EarthquakeData> GetEarthquakeDataAsync(decimal latitude, decimal longitude);
}