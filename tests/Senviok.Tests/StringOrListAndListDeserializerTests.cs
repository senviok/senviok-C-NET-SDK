using System.Text.Json;
using Senviok.Common;
using Senviok.Models.Audiences;

namespace Senviok.Tests;

public class StringOrListAndListDeserializerTests
{
    private class TestContainer
    {
        public StringOrList? Recipients { get; set; }
    }

    [Fact]
    public void StringOrList_HandlesImplicitConversions()
    {
        StringOrList single = "user@example.com";
        Assert.Single(single);
        Assert.Equal("user@example.com", single[0]);

        StringOrList multiple = new[] { "u1@example.com", "u2@example.com" };
        Assert.Equal(2, multiple.Count);
        Assert.Equal("u1@example.com", multiple[0]);
        Assert.Equal("u2@example.com", multiple[1]);
    }

    [Fact]
    public void StringOrList_SerializesSingleToString()
    {
        var container = new TestContainer { Recipients = "user@example.com" };
        var json = JsonSerializer.Serialize(container, JsonDefaults.Options);
        Assert.Equal("{\"recipients\":\"user@example.com\"}", json);
    }

    [Fact]
    public void StringOrList_SerializesMultipleToArray()
    {
        var container = new TestContainer { Recipients = new[] { "a@b.com", "c@d.com" } };
        var json = JsonSerializer.Serialize(container, JsonDefaults.Options);
        Assert.Equal("{\"recipients\":[\"a@b.com\",\"c@d.com\"]}", json);
    }

    [Fact]
    public void StringOrList_DeserializesFromString()
    {
        var json = "{\"recipients\":\"user@example.com\"}";
        var container = JsonSerializer.Deserialize<TestContainer>(json, JsonDefaults.Options);
        Assert.NotNull(container?.Recipients);
        Assert.Single(container!.Recipients!);
        Assert.Equal("user@example.com", container.Recipients![0]);
    }

    [Fact]
    public void StringOrList_DeserializesFromArray()
    {
        var json = "{\"recipients\":[\"a@b.com\",\"c@d.com\"]}";
        var container = JsonSerializer.Deserialize<TestContainer>(json, JsonDefaults.Options);
        Assert.NotNull(container?.Recipients);
        Assert.Equal(2, container!.Recipients!.Count);
        Assert.Equal("a@b.com", container.Recipients![0]);
        Assert.Equal("c@d.com", container.Recipients![1]);
    }

    [Fact]
    public void DeserializeList_NormalizesBareArray()
    {
        var json = "[{\"id\":\"1\",\"name\":\"List A\"},{\"id\":\"2\",\"name\":\"List B\"}]";
        var list = JsonDefaults.DeserializeList<Audience>(json);
        Assert.Equal(2, list.Count);
        Assert.Equal("List A", list[0].Name);
    }

    [Fact]
    public void DeserializeList_NormalizesDataEnvelope()
    {
        var json = "{\"data\":[{\"id\":\"1\",\"name\":\"List A\"}]}";
        var list = JsonDefaults.DeserializeList<Audience>(json);
        Assert.Single(list);
        Assert.Equal("List A", list[0].Name);
    }

    [Fact]
    public void DeserializeList_NormalizesLogsEnvelope()
    {
        var json = "{\"logs\":[{\"id\":\"1\",\"name\":\"List A\"}]}";
        var list = JsonDefaults.DeserializeList<Audience>(json);
        Assert.Single(list);
        Assert.Equal("List A", list[0].Name);
    }

    [Fact]
    public void DeserializeList_HandlesEmptyAndNullGracefully()
    {
        Assert.Empty(JsonDefaults.DeserializeList<Audience>(""));
        Assert.Empty(JsonDefaults.DeserializeList<Audience>("   "));
        Assert.Empty(JsonDefaults.DeserializeList<Audience>("{}"));
    }
}
