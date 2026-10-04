using System.Text.Json;
using System.Text.Json.Serialization;

namespace Senviok.Common;

/// <summary>
/// Shared System.Text.Json serializer configuration for the Senviok SDK.
/// </summary>
public static class JsonDefaults
{
    /// <summary>Default serializer options used for all requests and responses.</summary>
    public static readonly JsonSerializerOptions Options = CreateDefaultOptions();

    private static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };

        options.Converters.Add(new StringOrListJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }

    /// <summary>
    /// Deserializes a list response that might be wrapped in an envelope (e.g. {"data": [...]}, {"logs": [...]})
    /// or returned as a raw JSON array.
    /// </summary>
    public static IReadOnlyList<T> DeserializeList<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<T>();
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<T>>(json, Options) ?? (IReadOnlyList<T>)Array.Empty<T>();
        }

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("data", out var dataElem) && dataElem.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<List<T>>(dataElem.GetRawText(), Options) ?? (IReadOnlyList<T>)Array.Empty<T>();
            }

            if (root.TryGetProperty("logs", out var logsElem) && logsElem.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<List<T>>(logsElem.GetRawText(), Options) ?? (IReadOnlyList<T>)Array.Empty<T>();
            }

            if (root.TryGetProperty("items", out var itemsElem) && itemsElem.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<List<T>>(itemsElem.GetRawText(), Options) ?? (IReadOnlyList<T>)Array.Empty<T>();
            }
        }

        return Array.Empty<T>();
    }
}
