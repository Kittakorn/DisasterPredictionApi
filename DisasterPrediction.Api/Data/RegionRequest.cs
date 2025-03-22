using DisasterPrediction.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace DisasterPrediction.Api.Data;

public class RegionRequest : IValidatableObject
{
    [Required]
    [MaxLength(50)]
    public string RegionId { get; set; }

    [Required]
    public Coordinates LocationCoordinates { get; set; }

    [Required]
    [MinLength(1)]
    public string[] DisasterTypes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validDisasterTypes = new[] { "flood", "earthquake", "wildfire" };

        if (!DisasterTypes.All(x => validDisasterTypes.Contains(x)))
        {
            yield return new ValidationResult("Invalid disaster type", [nameof(DisasterTypes)]);
        }
    }
}
