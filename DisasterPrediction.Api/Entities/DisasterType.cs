using System;
using System.Collections.Generic;

namespace DisasterPrediction.Api.Entities;

public partial class DisasterType
{
    public int DisasterTypeId { get; set; }

    public string RegionId { get; set; }

    public string DisasterTypeName { get; set; }

    public int? ThresholdScore { get; set; }

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();

    public virtual Region Region { get; set; }
}
