using System.Net;
using System.Text;
using Senviok.Models.Emails;

namespace Senviok.Tests;

public class EmailServiceTests
{
    [Fact]
    public async Task UploadAttachmentThenSendUsesReturnedAttachmentId()
    {
        var requests = new List<(string Path, string ContentType, string Body)>();
        var handler = new TestHttpMessageHandler(req =>
        {
            var body = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult() ?? string.Empty;
            requests.Add((req.RequestUri!.AbsolutePath, req.Content?.Headers.ContentType?.MediaType ?? string.Empty, body));
            var response = req.RequestUri.AbsolutePath.EndsWith("/emails/attachments")
                ? "{\"id\":\"att_1\",\"filename\":\"invoice.pdf\",\"contentType\":\"application/pdf\",\"sizeBytes\":3,\"bucket\":\"test\",\"key\":\"test/att_1\"}"
                : "{\"id\":\"msg_1\"}";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test_key", new SenviokClientOptions { HttpMessageHandler = handler });
        var uploaded = await client.Emails.UploadAttachmentAsync("invoice.pdf", [1, 2, 3], "application/pdf");
        var sent = await client.Emails.SendAsync(new SendEmailRequest
        {
            From = "billing@example.com", To = "customer@example.com", Subject = "Invoice",
            AttachmentIds = [uploaded.Id]
        });

        Assert.Equal("msg_1", sent.Id);
        Assert.Equal(2, requests.Count);
        Assert.Equal("/v1/emails/attachments", requests[0].Path);
        Assert.Equal("multipart/form-data", requests[0].ContentType);
        Assert.True(requests[0].Body.Contains("name=file", StringComparison.OrdinalIgnoreCase)
            || requests[0].Body.Contains("name=\"file\"", StringComparison.OrdinalIgnoreCase));
        Assert.Contains("invoice.pdf", requests[0].Body);
        Assert.Equal("/v1/emails", requests[1].Path);
        Assert.Contains("\"attachmentIds\":[\"att_1\"]", requests[1].Body);
    }

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
