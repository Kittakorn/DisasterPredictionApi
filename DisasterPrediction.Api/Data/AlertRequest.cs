namespace DisasterPrediction.Api.Data
{
    public class AlertRequest
    {
        public string RegionId { get; set; }
        public string DisasterType { get; set; }
        public int ThresholdScore { get; set; }
    }
}
