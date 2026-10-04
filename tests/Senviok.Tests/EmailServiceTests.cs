using System.Net;
using Senviok.Models.Emails;

namespace Senviok.Tests;

public class EmailServiceTests
{
    [Fact]
    public async Task SendAsync_SendsPostRequestWithExpectedJsonPayload()
    {
        HttpRequestMessage? capturedRequest = null;
        string? capturedBody = null;

        var handler = new TestHttpMessageHandler(req =>
        {
            capturedRequest = req;
            capturedBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\": \"msg_email_123\"}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test_12345", new SenviokClientOptions
        {
            HttpMessageHandler = handler
        });

        var response = await client.Emails.SendAsync(new SendEmailRequest
        {
            From = "noreply@example.com",
            FromName = "Acme Team",
            To = "user@example.com",
            Subject = "Welcome aboard",
            Html = "<p>Welcome to Acme!</p>",
            Attachments = new List<EmailAttachment>
            {
                new() { Filename = "welcome.pdf", Content = "dGVzdA==", ContentType = "application/pdf" }
            }
        });

        Assert.NotNull(response);
        Assert.Equal("msg_email_123", response.Id);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.EndsWith("/v1/emails", capturedRequest.RequestUri!.AbsolutePath);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization?.Scheme);
        Assert.Equal("svk_test_12345", capturedRequest.Headers.Authorization?.Parameter);

        Assert.NotNull(capturedBody);
        Assert.Contains("\"from\":\"noreply@example.com\"", capturedBody);
        Assert.Contains("\"fromName\":\"Acme Team\"", capturedBody);
        Assert.Contains("\"to\":\"user@example.com\"", capturedBody);
        Assert.Contains("\"subject\":\"Welcome aboard\"", capturedBody);
        Assert.Contains("\"welcome.pdf\"", capturedBody);
    }

    [Fact]
    public async Task SendAsync_SupportsMultipleRecipients()
    {
        string? capturedBody = null;

        var handler = new TestHttpMessageHandler(req =>
        {
            capturedBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\": \"msg_multi_456\"}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test_12345", new SenviokClientOptions
        {
            HttpMessageHandler = handler
        });

        var response = await client.Emails.SendAsync(new SendEmailRequest
        {
            From = "team@example.com",
            To = new[] { "alice@example.com", "bob@example.com" },
            Subject = "Team Announcement",
            Html = "<p>All hands tomorrow.</p>"
        });

        Assert.Equal("msg_multi_456", response.Id);
        Assert.NotNull(capturedBody);
        Assert.Contains("[\"alice@example.com\",\"bob@example.com\"]", capturedBody);
    }
}
