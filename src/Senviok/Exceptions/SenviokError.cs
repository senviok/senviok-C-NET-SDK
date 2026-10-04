using System.Text.Json.Serialization;

namespace Senviok.Exceptions;

/// <summary>
/// Structured error payload returned by the Senviok API or ProblemDetails.
/// </summary>
public class SenviokError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonPropertyName("statusCode")]
    public int? StatusCode { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>>? Errors { get; set; }

    /// <summary>
    /// Gets the most descriptive error message available from the payload.
    /// </summary>
    public string GetBestMessage()
    {
        if (Errors != null && Errors.Count > 0)
        {
            var parts = Errors.Select(kv => $"{kv.Key}: {string.Join(", ", kv.Value)}");
            return string.Join("; ", parts);
        }

        if (!string.IsNullOrWhiteSpace(Message)) return Message!;
        if (!string.IsNullOrWhiteSpace(Detail)) return Detail!;
        if (!string.IsNullOrWhiteSpace(Error)) return Error!;
        if (!string.IsNullOrWhiteSpace(Title)) return Title!;

        return "Request failed.";
    }
}
