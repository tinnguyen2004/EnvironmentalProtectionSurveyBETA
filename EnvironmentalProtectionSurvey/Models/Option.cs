using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class Option
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public int? QuestionId { get; set; }

    public virtual ICollection<FilledSurvey> FilledSurveys { get; set; } = new List<FilledSurvey>();

    public virtual Question? Question { get; set; }
}
