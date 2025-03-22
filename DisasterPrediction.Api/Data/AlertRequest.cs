using System.ComponentModel.DataAnnotations;

namespace DisasterPrediction.Api.Data;

public class AlertRequest
{
    [Required]
    [MaxLength(50)]
    public string RegionId { get; set; }

    [Required]
    public string DisasterType { get; set; }

    [Required]
    [Range(1, 100)]
    public int ThresholdScore { get; set; }

}
