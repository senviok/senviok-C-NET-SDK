using System.Net;
using System.Security.Cryptography;
using System.Text;
using Senviok.Models.Webhooks;
using Senviok.Webhooks;

namespace Senviok.Tests;

public class WebhooksServiceTests
{
    [Fact]
    public async Task CreateAsync_CreatesWebhookSuccessfully()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler(req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\": \"whk_123\", \"url\": \"https://example.com/webhook\", \"secret\": \"whsec_abc\"}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var webhook = await client.Webhooks.CreateAsync("https://example.com/webhook", new[] { "email.delivered", "email.bounced" });

        Assert.Equal("whk_123", webhook.Id);
        Assert.Equal("https://example.com/webhook", webhook.Url);
        Assert.Equal("whsec_abc", webhook.Secret);
        Assert.NotNull(capturedRequest);
        Assert.EndsWith("/v1/webhooks", capturedRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public void VerifySignature_ReturnsTrueForValidHmac()
    {
        var secret = "whsec_supersecretkey123";
        var payload = "{\"event\":\"email.delivered\",\"id\":\"msg_123\"}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var validSignature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        var isValid = SenviokWebhooks.VerifySignature(payload, validSignature, secret);
        Assert.True(isValid);

        // Also test uppercase signature format tolerance
        Assert.True(SenviokWebhooks.VerifySignature(payload, validSignature.ToUpperInvariant(), secret));
    }

    [Fact]
    public void VerifySignature_ReturnsFalseForTamperedPayload()
    {
        var secret = "whsec_supersecretkey123";
        var payload = "{\"event\":\"email.delivered\",\"id\":\"msg_123\"}";
        var tamperedPayload = "{\"event\":\"email.delivered\",\"id\":\"msg_999\"}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        var isValid = SenviokWebhooks.VerifySignature(tamperedPayload, signature, secret);
        Assert.False(isValid);
    }

    [Fact]
    public void VerifySignature_ReturnsFalseForEmptyInputs()
    {
        Assert.False(SenviokWebhooks.VerifySignature("", "sig", "sec"));
        Assert.False(SenviokWebhooks.VerifySignature("body", "", "sec"));
        Assert.False(SenviokWebhooks.VerifySignature("body", "sig", ""));
        Assert.False(SenviokWebhooks.VerifySignature((string)null!, null, null));
    }
}
