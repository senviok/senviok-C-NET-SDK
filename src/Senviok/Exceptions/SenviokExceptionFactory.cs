using System.Net;
using System.Text.Json;
using Senviok.Common;

namespace Senviok.Exceptions;

internal static class SenviokExceptionFactory
{
    public static SenviokApiException Create(HttpResponseMessage response, string responseBody)
    {
        var statusCode = response.StatusCode;
        SenviokError? error = null;

        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            try
            {
                error = JsonSerializer.Deserialize<SenviokError>(responseBody, JsonDefaults.Options);
            }
            catch (JsonException)
            {
                // Non-JSON error body (e.g. proxy HTML or plain text)
            }
        }

        var message = error?.GetBestMessage();
        if (string.IsNullOrWhiteSpace(message) || message == "Request failed.")
        {
            message = !string.IsNullOrWhiteSpace(responseBody) && responseBody.Length < 300
                ? responseBody
                : $"The Senviok API returned HTTP {(int)statusCode} ({statusCode}).";
        }
        message ??= $"The Senviok API returned HTTP {(int)statusCode} ({statusCode}).";

        switch (statusCode)
        {
            case HttpStatusCode.BadRequest:
                return new SenviokValidationException(message, statusCode, responseBody, error);

            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
                return new SenviokAuthenticationException(message, statusCode, responseBody, error);

            case HttpStatusCode.NotFound:
                return new SenviokNotFoundException(message, statusCode, responseBody, error);

            case (HttpStatusCode)429:
                TimeSpan? retryAfter = null;
                if (response.Headers.RetryAfter != null)
                {
                    if (response.Headers.RetryAfter.Delta.HasValue)
                    {
                        retryAfter = response.Headers.RetryAfter.Delta.Value;
                    }
                    else if (response.Headers.RetryAfter.Date.HasValue)
                    {
                        var delta = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
                        if (delta > TimeSpan.Zero)
                        {
                            retryAfter = delta;
                        }
                    }
                }
                return new SenviokRateLimitException(message, statusCode, retryAfter, responseBody, error);

            default:
                return new SenviokApiException(message, statusCode, responseBody, error);
        }
    }
}
