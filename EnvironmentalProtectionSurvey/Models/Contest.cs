using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class Contest
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool? IsVisible { get; set; }

    public virtual ICollection<QuestionContest> QuestionContests { get; set; } = new List<QuestionContest>();

    public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();
}
