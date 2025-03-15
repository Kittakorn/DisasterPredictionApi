namespace DisasterPrediction.Api.Data
{
    public class AlertDto
    {
        public string RegionId { get; set; }
        public IEnumerable<AlertDetailDto> Alerts { get; set; }
    }
}
