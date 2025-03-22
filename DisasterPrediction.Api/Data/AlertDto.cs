namespace DisasterPrediction.Api.Data
{
    public class AlertDto
    {
        public string RegionId { get; set; }
        public string DisasterType { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
