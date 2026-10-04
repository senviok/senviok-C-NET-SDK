using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Senviok.Common;

/// <summary>
/// Represents a value that can be specified as either a single string or a list of strings
/// (such as recipient addresses 'to', 'cc', 'bcc', 'replyTo').
/// </summary>
[JsonConverter(typeof(StringOrListJsonConverter))]
public sealed class StringOrList : IReadOnlyList<string>, IEquatable<StringOrList>
{
    private readonly List<string> _items;

    public StringOrList()
    {
        _items = new List<string>();
    }

    public StringOrList(string single)
    {
        _items = string.IsNullOrWhiteSpace(single) ? new List<string>() : new List<string> { single };
    }

    public StringOrList(IEnumerable<string> items)
    {
        _items = items?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? new List<string>();
    }

    public int Count => _items.Count;

    public string this[int index] => _items[index];

    public IEnumerator<string> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    public static implicit operator StringOrList(string single) => new(single);

    public static implicit operator StringOrList(string[] items) => new(items);

    public static implicit operator StringOrList(List<string> items) => new(items);

    public override string ToString() => string.Join(", ", _items);

    public bool Equals(StringOrList? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _items.SequenceEqual(other._items);
    }

    public override bool Equals(object? obj) => obj is StringOrList other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 19;
            foreach (var item in _items)
            {
                hash = hash * 31 + (item?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}

public class StringOrListJsonConverter : JsonConverter<StringOrList>
{
    public override StringOrList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return new StringOrList();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            return str is null ? new StringOrList() : new StringOrList(str);
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var list = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    var item = reader.GetString();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        list.Add(item!);
                    }
                }
            }
            return new StringOrList(list);
        }

        return new StringOrList();
    }

    public override void Write(Utf8JsonWriter writer, StringOrList value, JsonSerializerOptions options)
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteNullValue();
            return;
        }

        if (value.Count == 1)
        {
            writer.WriteStringValue(value[0]);
        }
        else
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
        }
    }
}
