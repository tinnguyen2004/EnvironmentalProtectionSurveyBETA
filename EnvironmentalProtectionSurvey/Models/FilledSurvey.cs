using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class FilledSurvey
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UserId { get; set; }

    public int? SurveyId { get; set; }

    public int? OptionId { get; set; }

    public virtual Option? Option { get; set; }

    public virtual Survey? Survey { get; set; }

    public virtual User? User { get; set; }
}
