using DisasterPrediction.Api.Services;
using System.Net.Http.Headers;
using System.Text;

namespace DisasterPrediction.Api.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureHttpClient(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient<IOpenWeatherService, OpenWeatherService>(client =>
        {
            client.BaseAddress = new Uri(config["OpenWeather:BaseUrl"]);
        });

        services.AddHttpClient<IUsgsService, UsgsService>(client =>
        {
            client.BaseAddress = new Uri(config["USGS:BaseUrl"]);
        });

        services.AddHttpClient<IMailGunService, MailGunService>(client =>
        {
            var baseUrl = $"{config["MailGun:BaseUrl"]}{config["MailGun:Domain"]}/";
            var apiKey = Environment.GetEnvironmentVariable("MAILGUN_API_KEY");
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        });
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IDisasterAlertService, DisasterAlertService>();
        services.AddSingleton<IRiskCalculateService, RiskCalculateService>();
        services.AddSingleton<ITwilioService, TwilioService>();
    }
}
