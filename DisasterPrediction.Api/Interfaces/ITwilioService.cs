namespace DisasterPrediction.Api.Interfaces;

public interface ITwilioService
{
    Task SendSmsAsync(string to, string message);
}