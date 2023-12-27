using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class Question
{
    public int Id { get; set; }

    public int? SurveyId { get; set; }

    public string? Title { get; set; }

    public string? CorrectAnswer { get; set; }

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual Survey? Survey { get; set; }
}
