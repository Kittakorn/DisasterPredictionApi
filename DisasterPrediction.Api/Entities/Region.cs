using System;
using System.Collections.Generic;

namespace DisasterPrediction.Api.Entities;

public partial class Region
{
    public string RegionId { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public virtual ICollection<DisasterType> DisasterTypes { get; set; } = new List<DisasterType>();
}
