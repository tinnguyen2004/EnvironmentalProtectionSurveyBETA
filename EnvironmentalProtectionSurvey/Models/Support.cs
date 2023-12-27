using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class Support
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public string? TextMessage { get; set; }

    public string? Reply { get; set; }

    public virtual User? User { get; set; }
}
