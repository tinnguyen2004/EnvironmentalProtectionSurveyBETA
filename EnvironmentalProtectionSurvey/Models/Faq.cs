using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class Faq
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Answer { get; set; }
}
