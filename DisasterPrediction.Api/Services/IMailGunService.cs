namespace DisasterPrediction.Api.Services;

public interface IMailGunService
{
    Task SendEmailAsync(string to, string subject, string content);
}