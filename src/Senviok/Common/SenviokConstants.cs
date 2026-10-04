namespace Senviok;

/// <summary>
/// SDK-wide constant values for Senviok.
/// </summary>
public static class SenviokConstants
{
    /// <summary>Default production API endpoint.</summary>
    public const string DefaultBaseUrl = "https://api.senviok.live";

    /// <summary>Local development API endpoint.</summary>
    public const string LocalBaseUrl = "http://localhost:5033";

    /// <summary>Default client timeout in seconds.</summary>
    public const int DefaultTimeoutSeconds = 30;

    /// <summary>Default version prefix for API endpoints.</summary>
    public const string DefaultApiVersion = "v1";

    /// <summary>Header key for webhook signatures.</summary>
    public const string WebhookSignatureHeader = "X-Senviok-Signature";
}
