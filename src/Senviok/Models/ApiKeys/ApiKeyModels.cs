namespace Senviok.Models.ApiKeys;

/// <summary>
/// Newly generated API key credentials, returned only once upon creation.
/// </summary>
public class ApiKeyCreated
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    /// <summary>The full plaintext API token (e.g. "svk_live_..."). Store securely as it will not be shown again.</summary>
    public string Token { get; set; } = string.Empty;

    public string CreatedAt { get; set; } = string.Empty;
}

/// <summary>
/// Metadata for an existing API key.
/// </summary>
public class ApiKey
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string? CreatedAt { get; set; }
}

/// <summary>
/// Parameters for generating a new workspace API key.
/// </summary>
public class CreateApiKeyRequest
{
    /// <summary>Descriptive name for the API key (e.g. "Production Web App", "GitHub Action").</summary>
    public string Name { get; set; } = string.Empty;
}
