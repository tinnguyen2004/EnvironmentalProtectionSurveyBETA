using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class PasswordReset
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpiryTime { get; set; }

    public virtual User? User { get; set; }
}
