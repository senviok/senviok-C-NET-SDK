using System.Net;

namespace Senviok.Exceptions;

/// <summary>
/// Base exception class for all errors originating from the Senviok SDK.
/// </summary>
public class SenviokException : Exception
{
    /// <summary>The HTTP status code returned by the API, if available.</summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>The raw response body returned by the API, if available.</summary>
    public string? RawResponseBody { get; }

    /// <summary>The parsed error details returned by the API, if available.</summary>
    public SenviokError? ErrorDetails { get; }

    public SenviokException(string message) : base(message)
    {
    }

    public SenviokException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public SenviokException(string message, HttpStatusCode? statusCode, string? rawResponseBody = null, SenviokError? errorDetails = null)
        : base(message)
    {
        StatusCode = statusCode;
        RawResponseBody = rawResponseBody;
        ErrorDetails = errorDetails;
    }
}

/// <summary>
/// Thrown when the Senviok API returns an error HTTP status code.
/// </summary>
public class SenviokApiException : SenviokException
{
    public SenviokApiException(string message, HttpStatusCode statusCode, string? rawResponseBody = null, SenviokError? errorDetails = null)
        : base(message, statusCode, rawResponseBody, errorDetails)
    {
    }
}

/// <summary>
/// Thrown when authentication fails (HTTP 401 Unauthorized or 403 Forbidden).
/// </summary>
public class SenviokAuthenticationException : SenviokApiException
{
    public SenviokAuthenticationException(string message, HttpStatusCode statusCode, string? rawResponseBody = null, SenviokError? errorDetails = null)
        : base(message, statusCode, rawResponseBody, errorDetails)
    {
    }
}

/// <summary>
/// Thrown when the requested resource was not found (HTTP 404 Not Found).
/// </summary>
public class SenviokNotFoundException : SenviokApiException
{
    public SenviokNotFoundException(string message, HttpStatusCode statusCode, string? rawResponseBody = null, SenviokError? errorDetails = null)
        : base(message, statusCode, rawResponseBody, errorDetails)
    {
    }
}

/// <summary>
/// Thrown when input validation fails (HTTP 400 Bad Request).
/// </summary>
public class SenviokValidationException : SenviokApiException
{
    public SenviokValidationException(string message, HttpStatusCode statusCode, string? rawResponseBody = null, SenviokError? errorDetails = null)
        : base(message, statusCode, rawResponseBody, errorDetails)
    {
    }
}

/// <summary>
/// Thrown when API rate limits or quota are exceeded (HTTP 429 Too Many Requests).
/// </summary>
public class SenviokRateLimitException : SenviokApiException
{
    /// <summary>Number of seconds before the client should retry, if provided by the Retry-After header.</summary>
    public TimeSpan? RetryAfter { get; }

    public SenviokRateLimitException(string message, HttpStatusCode statusCode, TimeSpan? retryAfter = null, string? rawResponseBody = null, SenviokError? errorDetails = null)
        : base(message, statusCode, rawResponseBody, errorDetails)
    {
        RetryAfter = retryAfter;
    }
}
