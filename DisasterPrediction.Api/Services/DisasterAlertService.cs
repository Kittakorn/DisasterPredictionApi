using DisasterPrediction.Api.Data;
using DisasterPrediction.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace DisasterPrediction.Api.Services;

public class DisasterAlertService : IDisasterAlertService
{
    private readonly ApplicationDbContext _context;
    private readonly ITwilioService _twilioService;
    private readonly IOpenWeatherService _openWeatherService;
    private readonly IUsgsService _usgsService;
    private readonly IMailGunService _mailGunService;
    private readonly IRiskCalculateService _riskCalculateService;

    public DisasterAlertService(ApplicationDbContext context,
        ITwilioService twilioService,
        IOpenWeatherService openWeatherService,
        IUsgsService usgsService,
        IMailGunService mailGunService,
        IRiskCalculateService riskCalculateService)
    {
        _context = context;
        _twilioService = twilioService;
        _openWeatherService = openWeatherService;
        _usgsService = usgsService;
        _mailGunService = mailGunService;
        _riskCalculateService = riskCalculateService;
    }

    public async Task CreateRegionsAsync(IEnumerable<RegionRequest> requests)
    {
        var regions = new List<Region>();
        foreach (var request in requests)
        {
            var region = new Region
            {
                RegionId = request.RegionId,
                Latitude = request.LocationCoordinates.Latitude,
                Longitude = request.LocationCoordinates.Longitude,
                DisasterTypes = request.DisasterTypes
                    .Select(disasterType => new DisasterType
                    {
                        DisasterTypeName = disasterType,
                        RegionId = request.RegionId
                    })
                    .ToList()
            };

            regions.Add(region);
        }

        await _context.Regions.AddRangeAsync(regions);
        await _context.SaveChangesAsync();
    }

    public async Task SetAlertsAsync(IEnumerable<AlertRequest> requests)
    {
        foreach (var request in requests)
        {
            var disasterType = await GetDisasterType(request.RegionId, request.DisasterType);
            if (disasterType == null)
                throw new Exception("Disaster type not found");

            disasterType.ThresholdScore = request.ThresholdScore;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<AlertDto>> GetAlertsAsync()
    {
        var alerts = await _context.Alerts
            .AsNoTracking()
            .Include(alert => alert.DisasterType)
            .Select(alert => new AlertDto
            {
                RegionId = alert.DisasterType.RegionId,
                Alerts = alert.DisasterType.Alerts
                    .Select(alert => new AlertDetailDto
                    {
                        DisasterType = alert.DisasterType.DisasterTypeName,
                        RiskScore = alert.RiskScore,
                        RiskLevel = alert.RiskLevel
                    })
            })
            .ToListAsync();

        return alerts;
    }

    public async Task<List<DisasterRiskDto>> GetDisasterRisksAsync()
    {
        var regions = await GetRegionsAsync();
        var disasterRiskTasks = regions.Select(GetDisasterRisksForRegion);
        var disasterRisks = await Task.WhenAll(disasterRiskTasks);
        return disasterRisks.SelectMany(risks => risks).ToList();
    }

    public async Task SendAlertAsync(SendAlertRequest request)
    {
        var regions = await GetRegionsAsync();
        var disasterRiskTasks = regions.Select(GetDisasterRisksForRegion);
        var disasterRisks = await Task.WhenAll(disasterRiskTasks);

        var alerts = disasterRisks.SelectMany(risks => risks)
            .Where(risk => risk.AlertTriggered)
            .ToList();

        if (alerts.Count == 0)
            return;

        var message = string.Join("\n", alerts.Select(alert => $"{alert.DisasterType}: {alert.RiskLevel}"));

        if (!string.IsNullOrEmpty(request.PhoneNumber))
            await _twilioService.SendSmsAsync(request.PhoneNumber, "Alert\n" + message);

        if (!string.IsNullOrEmpty(request.Email))
            await _mailGunService.SendEmailAsync(request.Email, "Alert", message);

        await CreateAlertAsync(alerts);
    }

    private async Task<DisasterType> GetDisasterType(string regionId, string disasterTypeName)
    {
        var disasterType = await _context.DisasterTypes
            .Where(type => type.DisasterTypeName == disasterTypeName && type.RegionId == regionId)
            .FirstOrDefaultAsync();

        return disasterType;
    }

    private async Task<List<Region>> GetRegionsAsync()
    {
        var regions = await _context.Regions
            .AsNoTracking()
            .Include(region => region.DisasterTypes
                .Where(disaster => disaster.ThresholdScore.HasValue))
            .ToListAsync();

        return regions;
    }

    private async Task<List<DisasterRiskDto>> GetDisasterRisksForRegion(Region region)
    {
        var weatherData = _openWeatherService.GetWeatherDataAsync(region.Latitude, region.Longitude);
        var earthquakeData = _usgsService.GetEarthquakeDataAsync(region.Latitude, region.Longitude);
        await Task.WhenAll(weatherData, earthquakeData);

        var riskScores = _riskCalculateService.CalculateRiskScores(region, weatherData.Result, earthquakeData.Result);

        var disasterRisks = new List<DisasterRiskDto>();
        foreach (var risk in riskScores)
        {
            var disasterType = await GetDisasterType(region.RegionId, risk.Key);
            if (disasterType == null)
                throw new Exception("Disaster type not found");

            var riskLevel = _riskCalculateService.GetRiskLevel(risk.Value);
            var isAlertTriggered = _riskCalculateService.IsAlertTriggered(risk.Value, disasterType.ThresholdScore.Value);

            disasterRisks.Add(new DisasterRiskDto(disasterType.DisasterTypeId, region.RegionId, risk.Key, risk.Value, riskLevel, isAlertTriggered));
        }

        return disasterRisks;
    }

    private async Task CreateAlertAsync(List<DisasterRiskDto> disasterRisks)
    {
        var alerts = disasterRisks
            .Select(disasterRisk => new Alert
            {
                DisasterTypeId = disasterRisk.DisasterTypeId,
                RiskScore = disasterRisk.RiskScore,
                RiskLevel = disasterRisk.RiskLevel
            })
            .ToList();

        await _context.Alerts.AddRangeAsync(alerts);
        await _context.SaveChangesAsync();
    }
}
