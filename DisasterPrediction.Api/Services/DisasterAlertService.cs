using DisasterPrediction.Api.Data;
using DisasterPrediction.Api.Entities;
using DisasterPrediction.Api.Extensions;
using DisasterPrediction.Api.Handlers;
using DisasterPrediction.Api.Interfaces;
using DisasterPrediction.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace DisasterPrediction.Api.Services;

public class DisasterAlertService : IDisasterAlertService
{
    private readonly ApplicationDbContext _context;
    private readonly ITwilioService _twilioService;
    private readonly IOpenWeatherService _openWeatherService;
    private readonly IUsgsService _usgsService;
    private readonly IMailGunService _mailGunService;
    private readonly IRiskCalculateService _riskCalculateService;
    private readonly ILogger<DisasterAlertService> _logger;
    private readonly IDistributedCache _cache;

    public DisasterAlertService(ApplicationDbContext context,
        ITwilioService twilioService,
        IOpenWeatherService openWeatherService,
        IUsgsService usgsService,
        IMailGunService mailGunService,
        IRiskCalculateService riskCalculateService,
        ILogger<DisasterAlertService> logger,
        IDistributedCache cache)
    {
        _context = context;
        _twilioService = twilioService;
        _openWeatherService = openWeatherService;
        _usgsService = usgsService;
        _mailGunService = mailGunService;
        _riskCalculateService = riskCalculateService;
        _logger = logger;
        _cache = cache;
    }

    public async Task CreateRegionsAsync(IEnumerable<RegionRequest> requests)
    {
        foreach (var request in requests)
        {
            var region = await GetRegionByIdAsync(request.RegionId);
            if (region is null)
            {
                region = new Region
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

                await _context.Regions.AddAsync(region);
            }
            else
            {
                var existingDisasterTypes = region.DisasterTypes.Select(d => d.DisasterTypeName);

                var isDuplicate = request.DisasterTypes.Intersect(existingDisasterTypes).Any();
                if (isDuplicate)
                    throw new BadRequestException("Duplicate disaster types");

                foreach (var disasterType in request.DisasterTypes)
                {
                    region.DisasterTypes.Add(new DisasterType { DisasterTypeName = disasterType });
                }
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Regions created successfully for {Count} regions.", requests.Count());
    }

    public async Task SetAlertsAsync(IEnumerable<AlertRequest> requests)
    {
        foreach (var request in requests)
        {
            var disasterType = await GetDisasterTypeAsync(request.RegionId, request.DisasterType);
            if (disasterType == null)
                throw new BadRequestException("Disaster type not found");

            disasterType.ThresholdScore = request.ThresholdScore;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Alert settings updated successfully for {Count} alerts.", requests.Count());
    }

    public async Task<List<AlertDto>> GetAlertsAsync()
    {
        var cacheKey = "Alerts";
        var alerts = _cache.Get<List<AlertDto>>(cacheKey);
        if (alerts is null)
        {
            _logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database.", cacheKey);

            alerts = await _context.Alerts
                .AsNoTracking()
                .Select(alert => new AlertDto
                {
                    RegionId = alert.DisasterType.RegionId,
                    DisasterType = alert.DisasterType.DisasterTypeName,
                    RiskScore = alert.RiskScore,
                    RiskLevel = alert.RiskLevel,
                    CreateAt = alert.CreatedAt
                })
                .OrderBy(x => x.CreateAt)
                .ToListAsync();

            await _cache.SetAsync(cacheKey, alerts);
        }

        _logger.LogInformation("Successfully fetched {Count} alerts", alerts.Count);
        return alerts;
    }

    public async Task<List<DisasterRiskDto>> GetDisasterRisksAsync()
    {
        var cacheKey = "DisasterRisk";
        var disasterRisks = _cache.Get<List<DisasterRiskDto>>(cacheKey);
        if (disasterRisks is null)
        {
            _logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database.", cacheKey);

            var regions = await GetRegionsAsync();
            var disasterRiskTasks = await Task.WhenAll(regions.Select(GetDisasterRisksForRegion));
            disasterRisks = disasterRiskTasks.SelectMany(risk => risk).ToList();

            await _cache.SetAsync(cacheKey, disasterRisks);
            return disasterRisks;
        }

        _logger.LogInformation("Successfully fetched {Count} disaster risks", disasterRisks.Count);
        return disasterRisks;
    }

    public async Task SendAlertAsync(SendAlertRequest request)
    {
        var disasterRisks = await GetDisasterRisksAsync();
        var alerts = disasterRisks.Where(r => r.AlertTriggered).ToList();

        if (alerts.Count == 0)
        {
            _logger.LogInformation("No alerts triggered.");
            return;
        }

        var message = string.Join("\n", alerts.Select(alert => $"{alert.DisasterType}: {alert.RiskLevel}"));

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            await _twilioService.SendSmsAsync(request.PhoneNumber, "Alert\n" + message);
            _logger.LogInformation("Alert sent to {PhoneNumber}", request.PhoneNumber);
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            await _mailGunService.SendEmailAsync(request.Email, "Alert", message);
            _logger.LogInformation("Alert sent to {Email}", request.Email);
        }

        await CreateAlertAsync(alerts);
    }

    private async Task<DisasterType> GetDisasterTypeAsync(string regionId, string disasterTypeName)
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
            .Include(region => region.DisasterTypes)
            .ToListAsync();

        return regions;
    }

    private async Task<List<DisasterRiskDto>> GetDisasterRisksForRegion(Region region)
    {
        var weatherData = await _openWeatherService.GetWeatherDataAsync(region.RegionId);
        var earthquakeData = await _usgsService.GetEarthquakeDataAsync(region.Latitude, region.Longitude);

        if (weatherData is null)
            return [];

        var enviromentalData = new EnvironmentalData(weatherData, earthquakeData);
        var riskScores = _riskCalculateService.CalculateRiskScores(region, enviromentalData);
        var disasterRisks = new List<DisasterRiskDto>();
        var disasterTypes = region.DisasterTypes.ToDictionary(x => x.DisasterTypeName);

        foreach (var risk in riskScores)
        {
            if (!disasterTypes.TryGetValue(risk.Key, out var disasterType))
                throw new BadRequestException("Disaster type not found");

            var riskLevel = _riskCalculateService.GetRiskLevel(risk.Value);
            var isAlertTriggered = _riskCalculateService.IsAlertTriggered(risk.Value, disasterType.ThresholdScore);

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

        _logger.LogInformation("Alerts created successfully for {Count} alerts.", alerts.Count);
    }

    private async Task<Region> GetRegionByIdAsync(string regionId)
    {
        return await _context.Regions
            .Include(x => x.DisasterTypes)
            .Where(disaster => disaster.RegionId == regionId)
            .FirstOrDefaultAsync();
    }
}
