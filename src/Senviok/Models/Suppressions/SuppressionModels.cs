namespace Senviok.Models.Suppressions;

/// <summary>
/// A suppressed email address (bounced, complained, or unsubscribed).
/// </summary>
public class Suppression
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? CreatedAt { get; set; }
}

/// <summary>
/// Parameters for adding an email to the workspace suppression list.
/// </summary>
public class CreateSuppressionRequest
{
    /// <summary>The email address to suppress.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Reason for suppression (e.g. "unsubscribe", "bounce", "complaint", "manual").</summary>
    public string? Reason { get; set; }
}
