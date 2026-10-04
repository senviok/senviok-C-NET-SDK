namespace Senviok.Models.Audiences;

/// <summary>
/// A subscriber audience list.
/// </summary>
public class Audience
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CreatedAt { get; set; }
}

/// <summary>
/// Parameters for creating a new audience.
/// </summary>
public class CreateAudienceRequest
{
    /// <summary>Name of the audience list (e.g. "Newsletter Subscribers").</summary>
    public string Name { get; set; } = string.Empty;
}
