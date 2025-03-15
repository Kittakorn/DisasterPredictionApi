
using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Services;

public interface IUsgsService
{
    Task<EarthquakeData> GetEarthquakeDataAsync(decimal latitude, decimal longitude);
}