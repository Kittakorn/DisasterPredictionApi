
using DisasterPrediction.Api.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace DisasterPrediction.Api.Services;

public class TwilioService : ITwilioService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromPhoneNumber;

    public TwilioService()
    {
        _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
        _fromPhoneNumber = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER");

        TwilioClient.Init(_accountSid, _authToken);
    }


    public async Task SendSmsAsync(string toPhoneNumber, string body)
    {
        var message = await MessageResource.CreateAsync(
            body: body,
            from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
            to: new Twilio.Types.PhoneNumber(toPhoneNumber)
        );

        if (message.ErrorCode.HasValue)
            throw new InvalidOperationException($"Failed to send SMS: {message.ErrorMessage}");
    }

}