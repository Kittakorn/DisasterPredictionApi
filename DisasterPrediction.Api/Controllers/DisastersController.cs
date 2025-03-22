using DisasterPrediction.Api.Data;
using DisasterPrediction.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DisasterPrediction.Api.Controllers;

[Route("api")]
[ApiController]
public class DisastersController : ControllerBase
{
    private readonly IDisasterAlertService _disasterAlertService;

    public DisastersController(IDisasterAlertService disasterAlertService)
    {
        _disasterAlertService = disasterAlertService;
    }

    [HttpPost("regions")]
    public async Task<IActionResult> CreateRegions(IEnumerable<RegionRequest> requests)
    {
        await _disasterAlertService.CreateRegionsAsync(requests);
        return Ok(new { message = "Regions created successfully" });
    }


    [HttpPost("alert-settings")]
    public async Task<IActionResult> AlertSettings(IEnumerable<AlertRequest> requests)
    {
        await _disasterAlertService.SetAlertsAsync(requests);
        return Ok(new { message = "Alert settings updated successfully" });
    }

    [HttpGet("disaster-risks")]
    public async Task<IActionResult> DisasterRisk()
    {
        return Ok(await _disasterAlertService.GetDisasterRisksAsync());
    }

    [HttpPost("alert/send")]
    public async Task<IActionResult> SendAlert(SendAlertRequest request)
    {
        await _disasterAlertService.SendAlertAsync(request);
        return Ok(new { message = "Alert sent successfully" });
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> Alerts()
    {
        return Ok(await _disasterAlertService.GetAlertsAsync());
    }
}
