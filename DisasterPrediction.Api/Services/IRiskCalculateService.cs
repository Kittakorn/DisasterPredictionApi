using DisasterPrediction.Api.Entities;
using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Services;

public interface IRiskCalculateService
{
    Dictionary<string, int> CalculateRiskScores(Region region, WeatherData weatherData, EarthquakeData earthquakeData);
    string GetRiskLevel(int riskScore);
    bool IsAlertTriggered(int riskScore, decimal thresholdScore);
}