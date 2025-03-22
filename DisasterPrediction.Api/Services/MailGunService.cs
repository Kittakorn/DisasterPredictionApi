using DisasterPrediction.Api.Interfaces;

namespace DisasterPrediction.Api.Services;

public class MailGunService : IMailGunService
{
    private readonly HttpClient _client;
    private readonly string _domain;

    public MailGunService(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _domain = configuration["MailGun:Domain"];
    }

    public async Task SendEmailAsync(string to, string subject, string content)
    {
        var form = new Dictionary<string, string>
        {
            ["from"] = $"Disaster Prediction <postmaster@{_domain}>",
            ["to"] = to,
            ["subject"] = "Alert",
            ["text"] = content
        };

        var response = await _client.PostAsync("messages", new FormUrlEncodedContent(form));
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Failed to send email: {response.StatusCode}");
    }
}