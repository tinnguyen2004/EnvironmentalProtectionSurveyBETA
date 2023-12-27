using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class Winner
{
    public int Id { get; set; }

    public int? ContestId { get; set; }

    public int? UserId { get; set; }

    public virtual Contest? Contest { get; set; }

    public virtual User? User { get; set; }
}
