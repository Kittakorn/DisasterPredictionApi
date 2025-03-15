using System.Text.Json.Serialization;

namespace DisasterPrediction.Api.Data;

public class DisasterRiskDto
{
    public DisasterRiskDto(int disasterTypeId, string regionId, string disasterType, int riskScore, string riskLevel, bool alertTriggered)
    {
        DisasterTypeId = disasterTypeId;
        RegionId = regionId;
        DisasterType = disasterType;
        RiskScore = riskScore;
        RiskLevel = riskLevel;
        AlertTriggered = alertTriggered;
    }

    [JsonIgnore]
    public int DisasterTypeId { get; set; }
    public string RegionId { get; set; }
    public string DisasterType { get; set; }
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; }
    public bool AlertTriggered { get; set; }
}
