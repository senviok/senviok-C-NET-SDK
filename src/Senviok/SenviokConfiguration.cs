namespace Senviok;

/// <summary>
/// Per-request configuration options that override client-level defaults.
/// </summary>
public class SenviokRequestOptions
{
    /// <summary>Override the API key for this specific request.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Optional idempotency key to prevent duplicate processing.</summary>
    public string? IdempotencyKey { get; set; }

    /// <summary>Additional custom headers to include with the request.</summary>
    public Dictionary<string, string>? CustomHeaders { get; set; }
}

/// <summary>
/// Ambient / static fallback configuration for the Senviok SDK.
/// </summary>
public static class SenviokConfiguration
{
    private static string? _apiKey;
    private static string _baseUrl = SenviokConstants.DefaultBaseUrl;
    private static TimeSpan _timeout = TimeSpan.FromSeconds(SenviokConstants.DefaultTimeoutSeconds);

    /// <summary>Default API key used when none is provided to <see cref="SenviokClient"/>.</summary>
    public static string? ApiKey
    {
        get => _apiKey ?? Environment.GetEnvironmentVariable("SENVIOK_API_KEY");
        set => _apiKey = value;
    }

    /// <summary>Default base URL for requests.</summary>
    public static string BaseUrl
    {
        get => _baseUrl;
        set => _baseUrl = value ?? SenviokConstants.DefaultBaseUrl;
    }

    /// <summary>Default timeout for HTTP requests.</summary>
    public static TimeSpan Timeout
    {
        get => _timeout;
        set => _timeout = value;
    }
}
