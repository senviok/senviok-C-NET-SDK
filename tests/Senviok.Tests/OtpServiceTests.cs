using System.Net;
using Senviok.Models.Otp;

namespace Senviok.Tests;

public class OtpServiceTests
{
    [Fact]
    public async Task SendOtpAsync_SendsExpectedPayload()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler(req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"messageId\": \"otp_msg_1\", \"to\": \"+1999999999\", \"expiresAt\": \"2026-10-03T12:00:00Z\"}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test_otp", new SenviokClientOptions { HttpMessageHandler = handler });

        var response = await client.Otp.SendAsync(new SendOtpRequest
        {
            To = "+1999999999",
            From = "AcmeAuth",
            Channel = "SMS",
            Length = 6,
            ExpiryMinutes = 10
        });

        Assert.Equal("otp_msg_1", response.MessageId);
        Assert.Equal("+1999999999", response.To);
        Assert.NotNull(capturedRequest);
        Assert.EndsWith("/v1/otp/send", capturedRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task VerifyOtpAsync_ReturnsVerificationStatus()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler(req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"verified\": true, \"message\": \"PIN successfully verified.\"}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test_otp", new SenviokClientOptions { HttpMessageHandler = handler });

        var response = await client.Otp.VerifyAsync("+1999999999", "123456");

        Assert.True(response.Verified);
        Assert.Equal("PIN successfully verified.", response.Message);
        Assert.NotNull(capturedRequest);
        Assert.EndsWith("/v1/otp/verify", capturedRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task ResendOtpAsync_SendsExpectedPayload()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler(req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"messageId\": \"otp_msg_2\", \"to\": \"+1999999999\", \"resent\": true}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test_otp", new SenviokClientOptions { HttpMessageHandler = handler });

        var response = await client.Otp.ResendAsync(new ResendOtpRequest
        {
            To = "+1999999999",
            From = "AcmeAuth"
        });

        Assert.True(response.Resent);
        Assert.Equal("otp_msg_2", response.MessageId);
        Assert.NotNull(capturedRequest);
        Assert.EndsWith("/v1/otp/resend", capturedRequest!.RequestUri!.AbsolutePath);
    }
}
