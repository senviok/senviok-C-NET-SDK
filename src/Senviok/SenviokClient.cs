using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Senviok.Common;
using Senviok.Exceptions;
using Senviok.Services;

namespace Senviok;

/// <summary>
/// The primary entry point for communicating with the Senviok API.
/// </summary>
public sealed class SenviokClient : ISenviokClient
{
    private readonly HttpClient _http;
    private readonly bool _disposeHttpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    /// <inheritdoc/>
    public IEmailsResource Emails { get; }

    /// <inheritdoc/>
    public ISmsResource Sms { get; }

    /// <inheritdoc/>
    public IWhatsAppResource WhatsApp { get; }

    /// <inheritdoc/>
    public IOtpResource Otp { get; }

    /// <inheritdoc/>
    public IMessagesResource Messages { get; }

    /// <inheritdoc/>
    public IWebhooksResource Webhooks { get; }

    /// <inheritdoc/>
    public ITemplatesResource Templates { get; }

    /// <inheritdoc/>
    public IDomainsResource Domains { get; }

    /// <inheritdoc/>
    public IAudiencesResource Audiences { get; }

    /// <inheritdoc/>
    public IContactsResource Contacts { get; }

    /// <inheritdoc/>
    public ISuppressionsResource Suppressions { get; }

    /// <inheritdoc/>
    public IApiKeysResource ApiKeys { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SenviokClient"/> class.
    /// </summary>
    /// <param name="apiKey">The Senviok API key (e.g. svk_live_... or svk_test_...). If null, resolves from options or environment variable.</param>
    /// <param name="options">Optional client configuration.</param>
    public SenviokClient(string? apiKey = null, SenviokClientOptions? options = null)
    {
        options ??= new SenviokClientOptions();

        var resolvedKey = apiKey ?? options.ApiKey ?? SenviokConfiguration.ApiKey;
        if (string.IsNullOrWhiteSpace(resolvedKey))
        {
            throw new ArgumentException(
                "A Senviok API key is required. Pass it to the SenviokClient constructor, configure SenviokClientOptions.ApiKey, or set the SENVIOK_API_KEY environment variable.",
                nameof(apiKey));
        }

        _apiKey = resolvedKey!;

        var baseAddress = (options.BaseUrl ?? SenviokConfiguration.BaseUrl).TrimEnd('/');
        _baseUrl = baseAddress.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
            ? baseAddress + "/"
            : baseAddress + "/v1/";

        if (options.HttpClient != null)
        {
            _http = options.HttpClient;
            _disposeHttpClient = false;
        }
        else if (options.HttpMessageHandler != null)
        {
            _http = new HttpClient(options.HttpMessageHandler)
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = options.Timeout
            };
            _disposeHttpClient = true;
        }
        else
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = options.Timeout
            };
            _disposeHttpClient = true;
        }

        Emails = new EmailsResource(this);
        Sms = new SmsResource(this);
        WhatsApp = new WhatsAppResource(this);
        Otp = new OtpResource(this);
        Messages = new MessagesResource(this);
        Webhooks = new WebhooksResource(this);
        Templates = new TemplatesResource(this);
        Domains = new DomainsResource(this);
        Audiences = new AudiencesResource(this);
        Contacts = new ContactsResource(this);
        Suppressions = new SuppressionsResource(this);
        ApiKeys = new ApiKeysResource(this);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SenviokClient"/> using an existing <see cref="HttpClient"/>.
    /// </summary>
    public SenviokClient(HttpClient httpClient, string? apiKey = null)
        : this(apiKey, new SenviokClientOptions { HttpClient = httpClient })
    {
    }

    internal async Task<T> SendAsync<T>(
        HttpMethod method,
        string path,
        SenviokRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(method, path, requestOptions);
        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var body = await ReadContentAsync(response, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw SenviokExceptionFactory.Create(response, body);
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return default!;
        }

        return JsonSerializer.Deserialize<T>(body, JsonDefaults.Options)
            ?? throw new SenviokApiException("Failed to deserialize API response.", response.StatusCode, body);
    }

    internal async Task<TResponse> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string path,
        TRequest body,
        SenviokRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(method, path, requestOptions);
        var json = JsonSerializer.Serialize(body, JsonDefaults.Options);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await ReadContentAsync(response, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw SenviokExceptionFactory.Create(response, responseBody);
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return default!;
        }

        return JsonSerializer.Deserialize<TResponse>(responseBody, JsonDefaults.Options)
            ?? throw new SenviokApiException("Failed to deserialize API response.", response.StatusCode, responseBody);
    }

    internal async Task<IReadOnlyList<T>> SendListAsync<T>(
        HttpMethod method,
        string path,
        IDictionary<string, string>? query = null,
        SenviokRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var finalPath = BuildPathWithQuery(path, query);
        using var request = CreateRequest(method, finalPath, requestOptions);
        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var body = await ReadContentAsync(response, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw SenviokExceptionFactory.Create(response, body);
        }

        return JsonDefaults.DeserializeList<T>(body);
    }

    internal async Task SendNoContentAsync(
        HttpMethod method,
        string path,
        SenviokRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(method, path, requestOptions);
        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var body = await ReadContentAsync(response, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw SenviokExceptionFactory.Create(response, body);
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string path, SenviokRequestOptions? options)
    {
        var cleanPath = path.TrimStart('/');
        var request = new HttpRequestMessage(method, cleanPath);

        var key = options?.ApiKey ?? _apiKey;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.TryAddWithoutValidation("User-Agent", "senviok-dotnet/1.0.0");

        if (options != null)
        {
            if (!string.IsNullOrWhiteSpace(options.IdempotencyKey))
            {
                request.Headers.TryAddWithoutValidation("Idempotency-Key", options.IdempotencyKey);
            }

            if (options.CustomHeaders != null)
            {
                foreach (var header in options.CustomHeaders)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        return request;
    }

    private static string BuildPathWithQuery(string path, IDictionary<string, string>? query)
    {
        if (query == null || query.Count == 0)
        {
            return path;
        }

        var sb = new StringBuilder(path);
        sb.Append(path.Contains("?") ? "&" : "?");

        bool first = true;
        foreach (var kvp in query)
        {
            if (string.IsNullOrWhiteSpace(kvp.Value)) continue;
            if (!first) sb.Append('&');
            sb.Append(Uri.EscapeDataString(kvp.Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(kvp.Value));
            first = false;
        }

        return sb.ToString();
    }

    private static async Task<string> ReadContentAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content == null) return string.Empty;
#if NET8_0_OR_GREATER
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
    }

    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _http.Dispose();
        }
    }
}
