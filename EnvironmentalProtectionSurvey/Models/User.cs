using System;
using System.Collections.Generic;

namespace EnvironmentalProtectionSurvey.Models;

public partial class User
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? NumberCode { get; set; }

    public string? Class { get; set; }

    public string? Specification { get; set; }

    public string? Section { get; set; }

    public DateTime? JoinDate { get; set; }

    public string? Role { get; set; }

    public int? Active { get; set; }

    public DateTime? ExpiryTime { get; set; }

    public string? Token { get; set; }

    public virtual ICollection<Award> Awards { get; set; } = new List<Award>();

    public virtual ICollection<FilledSurvey> FilledSurveys { get; set; } = new List<FilledSurvey>();

    public virtual ICollection<ForgotPassword> ForgotPasswords { get; set; } = new List<ForgotPassword>();

    public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();

    public virtual ICollection<Support> Supports { get; set; } = new List<Support>();

    public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();
}
