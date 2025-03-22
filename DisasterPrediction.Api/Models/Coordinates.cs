using System.ComponentModel.DataAnnotations;

namespace DisasterPrediction.Api.Models;

public class Coordinates
{
    [Required]
    public decimal Latitude { get; set; }

    [Required]
    public decimal Longitude { get; set; }
}
