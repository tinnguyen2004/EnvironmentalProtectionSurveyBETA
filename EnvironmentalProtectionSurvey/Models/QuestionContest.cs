using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class QuestionContest
{
    public int Id { get; set; }

    public int? ContestId { get; set; }

    public string QuestionText { get; set; } = null!;

    //public string AnswerOptions { get; set; } = null!;
    public List<string> AnswerOptions { get; set; } = new List<string>();


    public string CorrectAnswer { get; set; } = null!;

    public virtual Contest? Contest { get; set; }
}
