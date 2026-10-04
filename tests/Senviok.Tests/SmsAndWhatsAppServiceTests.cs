using System.Net;
using Senviok.Models.Sms;
using Senviok.Models.WhatsApp;

namespace Senviok.Tests;

public class SmsAndWhatsAppServiceTests
{
    [Fact]
    public async Task SendSmsAsync_DispatchesExpectedRequest()
    {
        HttpRequestMessage? capturedRequest = null;
        string? capturedBody = null;

        var handler = new TestHttpMessageHandler(req =>
        {
            capturedRequest = req;
            capturedBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\": \"sms_msg_789\", \"count\": 1}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test_sms", new SenviokClientOptions
        {
            HttpMessageHandler = handler
        });

        var response = await client.Sms.SendAsync("+1234567890", "AcmeAlert", "Your security code is 884920.");

        Assert.Equal("sms_msg_789", response.Id);
        Assert.Equal(1, response.Count);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.EndsWith("/v1/sms", capturedRequest.RequestUri!.AbsolutePath);

        Assert.NotNull(capturedBody);
        Assert.Contains("\"to\":\"+1234567890\"", capturedBody);
        Assert.Contains("\"from\":\"AcmeAlert\"", capturedBody);
        Assert.Contains("\"text\":\"Your security code is 884920.\"", capturedBody);
    }

    [Fact]
    public async Task SendWhatsAppAsync_DispatchesExpectedRequest()
    {
        HttpRequestMessage? capturedRequest = null;
        string? capturedBody = null;

        var handler = new TestHttpMessageHandler(req =>
        {
            capturedRequest = req;
            capturedBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\": \"wa_msg_101\"}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test_wa", new SenviokClientOptions
        {
            HttpMessageHandler = handler
        });

        var response = await client.WhatsApp.SendAsync(new SendWhatsAppRequest
        {
            To = "+2348012345678",
            From = "SenviokWA",
            Text = "Order confirmation message",
            TemplateId = "tmpl_wa_order",
            Data = new Dictionary<string, object> { ["customer_name"] = "Alice" }
        });

        Assert.Equal("wa_msg_101", response.Id);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
        Assert.EndsWith("/v1/whatsapp", capturedRequest.RequestUri!.AbsolutePath);

        Assert.NotNull(capturedBody);
        Assert.Contains("\"templateId\":\"tmpl_wa_order\"", capturedBody);
        Assert.Contains("\"customer_name\":\"Alice\"", capturedBody);
    }
}
