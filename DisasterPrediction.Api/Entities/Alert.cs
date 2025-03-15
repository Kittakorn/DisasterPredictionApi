using System;
using System.Collections.Generic;

namespace DisasterPrediction.Api.Entities;

public partial class Alert
{
    public int AlertId { get; set; }

    public int DisasterTypeId { get; set; }

    public int RiskScore { get; set; }

    public string RiskLevel { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual DisasterType DisasterType { get; set; }
}
