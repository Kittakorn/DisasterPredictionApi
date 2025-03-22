using DisasterPrediction.Api.Data;

namespace DisasterPrediction.Api.Interfaces;

public interface IDisasterAlertService
{
    Task CreateRegionsAsync(IEnumerable<RegionRequest> requests);
    Task SetAlertsAsync(IEnumerable<AlertRequest> requests);
    Task<List<DisasterRiskDto>> GetDisasterRisksAsync();
    Task SendAlertAsync(SendAlertRequest request);
    Task<List<AlertDto>> GetAlertsAsync();
}