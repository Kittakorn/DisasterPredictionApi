using DisasterPrediction.Api.Models;

namespace DisasterPrediction.Api.Data;

public class RegionRequest
{
    public string RegionId { get; set; }
    public Coordinates LocationCoordinates { get; set; }
    public string[] DisasterTypes { get; set; }
}
