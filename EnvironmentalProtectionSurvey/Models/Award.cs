using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class Award
{
    public int Id { get; set; }

    public int? SurveyId { get; set; }

    public int? UserId { get; set; }

    public string? Bonus { get; set; }

    public virtual Survey? Survey { get; set; }

    public virtual User? User { get; set; }
}
