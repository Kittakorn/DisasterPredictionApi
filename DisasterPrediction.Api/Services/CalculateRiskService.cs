using DisasterPrediction.Api.Entities;
using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Services;

public class RiskCalculateService : IRiskCalculateService
{
    public Dictionary<string, int> CalculateRiskScores(Region region, WeatherData weatherData, EarthquakeData earthquakeData)
    {
        var risks = new Dictionary<string, int>();

        foreach (var disasterType in region.DisasterTypes)
        {
            var score = disasterType.DisasterTypeName switch
            {
                "flood" => CalculateFloodRisk(weatherData.Rain.RainFall),
                "wildfire" => CalculateWildfireRisk(weatherData.Main.Temp, weatherData.Main.Humidity),
                "earthquake" => CalculateEarthquakeRisk(0),
                _ => throw new ArgumentException("Invalid disaster type")
            };

            risks.Add(disasterType.DisasterTypeName, score);
        }

        return risks;
    }

    public int CalculateFloodRisk(double rainfall)
    {
        return 0;
    }

    public int CalculateEarthquakeRisk(double magnitude)
    {
        return 0;
    }

    public int CalculateWildfireRisk(double temperature, int humidity)
    {
        return 0;
    }

    public string GetRiskLevel(int riskScore)
    {

        var riskLevel = riskScore switch
        {
            >= 80 => "High",
            >= 50 => "Medium",
            _ => "Low",
        };

        return riskLevel;
    }

    public bool IsAlertTriggered(int riskScore, decimal thresholdScore)
    {
        return riskScore >= thresholdScore;
    }
}
