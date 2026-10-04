namespace Senviok.Models.Domains;

/// <summary>
/// A sending domain configured in Senviok.
/// </summary>
public class Domain
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CreatedAt { get; set; }
}

/// <summary>
/// Parameters for adding a new sending domain.
/// </summary>
public class CreateDomainRequest
{
    /// <summary>The domain or subdomain to register (e.g. "mail.example.com").</summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DNS DKIM token record details for domain authentication.
/// </summary>
public class DkimToken
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool? Status { get; set; }
}

/// <summary>
/// Response containing DKIM DNS records required for domain verification.
/// </summary>
public class DkimResponse
{
    public string Status { get; set; } = string.Empty;
    public List<DkimToken> Tokens { get; set; } = new();
}

/// <summary>
/// Result of triggering domain DNS verification check.
/// </summary>
public class VerifyDomainResponse
{
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public Dictionary<string, object>? Diagnostics { get; set; }
}
