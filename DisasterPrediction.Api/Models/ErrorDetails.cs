using System.Text.Json.Serialization;

namespace DisasterPrediction.Api.Models;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>(StringComparer.Ordinal);
}
