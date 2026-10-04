namespace Senviok;

/// <summary>
/// Configuration options for initializing a <see cref="SenviokClient"/>.
/// </summary>
public class SenviokClientOptions
{
    /// <summary>
    /// The API key used to authenticate requests (starting with svk_live_ or svk_test_).
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// The base API URL. Defaults to <see cref="SenviokConstants.DefaultBaseUrl"/> ("https://api.senviok.live").
    /// For local testing, set to <see cref="SenviokConstants.LocalBaseUrl"/> ("http://localhost:5033").
    /// </summary>
    public string BaseUrl { get; set; } = SenviokConstants.DefaultBaseUrl;

    /// <summary>
    /// Request timeout. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(SenviokConstants.DefaultTimeoutSeconds);

    /// <summary>
    /// Optional custom <see cref="HttpMessageHandler"/> (useful for mock testing or custom proxies).
    /// </summary>
    public HttpMessageHandler? HttpMessageHandler { get; set; }

    /// <summary>
    /// Optional pre-configured <see cref="System.Net.Http.HttpClient"/> instance to use for all requests.
    /// </summary>
    public HttpClient? HttpClient { get; set; }
}
