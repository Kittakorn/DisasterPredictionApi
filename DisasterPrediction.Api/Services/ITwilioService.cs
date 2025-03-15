namespace DisasterPrediction.Api.Services;

public interface ITwilioService
{
    Task SendSmsAsync(string to, string message);
}