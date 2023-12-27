using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class Survey
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? UserType { get; set; }

    public string? Form { get; set; }

    public int? UserPost { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? EndAt { get; set; }

    public bool? IsVisible { get; set; }

    public virtual ICollection<Award> Awards { get; set; } = new List<Award>();

    public virtual ICollection<FilledSurvey> FilledSurveys { get; set; } = new List<FilledSurvey>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
