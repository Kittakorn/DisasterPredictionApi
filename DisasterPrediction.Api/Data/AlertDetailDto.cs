namespace DisasterPrediction.Api.Data
{
    public class AlertDetailDto
    {
        public string DisasterType { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
