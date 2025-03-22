using DisasterPrediction.Api.Entities;
using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Interfaces;

public interface IRiskCalculateService
{
    Dictionary<string, int> CalculateRiskScores(Region region, EnvironmentalData environmentalData);
    string GetRiskLevel(int riskScore);
    bool IsAlertTriggered(int riskScore, int? thresholdScore);
}