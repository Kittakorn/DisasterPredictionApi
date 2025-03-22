namespace DisasterPrediction.Api.Interfaces;

public interface IMailGunService
{
    Task SendEmailAsync(string to, string subject, string content);
}