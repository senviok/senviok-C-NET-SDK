using System.Net;
using Senviok.Exceptions;
using Senviok.Models.Emails;

namespace Senviok.Tests;

public class ExceptionHandlingTests
{
    [Fact]
    public async Task Http401_ThrowsSenviokAuthenticationException()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.Unauthorized, "{\"message\": \"Invalid API key\"}");
        using var client = new SenviokClient("svk_bad_key", new SenviokClientOptions { HttpMessageHandler = handler });

        var ex = await Assert.ThrowsAsync<SenviokAuthenticationException>(() =>
            client.Emails.SendAsync(new SendEmailRequest { To = "a@b.com", Subject = "Hi" }));

        Assert.Equal(HttpStatusCode.Unauthorized, ex.StatusCode);
        Assert.Equal("Invalid API key", ex.Message);
        Assert.Equal("Invalid API key", ex.ErrorDetails?.Message);
    }

    [Fact]
    public async Task Http400_ThrowsSenviokValidationException_WithFieldErrors()
    {
        var json = "{\"title\":\"One or more validation errors occurred.\",\"errors\":{\"To\":[\"The To field is required.\"]}}";
        var handler = new TestHttpMessageHandler(HttpStatusCode.BadRequest, json);
        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var ex = await Assert.ThrowsAsync<SenviokValidationException>(() =>
            client.Emails.SendAsync(new SendEmailRequest { Subject = "Hi" }));

        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Contains("To: The To field is required.", ex.Message);
        Assert.NotNull(ex.ErrorDetails?.Errors);
        Assert.True(ex.ErrorDetails!.Errors!.ContainsKey("To"));
    }

    [Fact]
    public async Task Http404_ThrowsSenviokNotFoundException()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.NotFound, "{\"message\": \"Template not found\"}");
        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var ex = await Assert.ThrowsAsync<SenviokNotFoundException>(() =>
            client.Templates.GetAsync("tmpl_missing"));

        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal("Template not found", ex.Message);
    }

    [Fact]
    public async Task Http429_ThrowsSenviokRateLimitException_WithRetryAfter()
    {
        var handler = new TestHttpMessageHandler(_ =>
        {
            var resp = new HttpResponseMessage((HttpStatusCode)429)
            {
                Content = new StringContent("{\"message\": \"Too many requests. Please slow down.\"}", System.Text.Encoding.UTF8, "application/json")
            };
            resp.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(60));
            return resp;
        });

        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var ex = await Assert.ThrowsAsync<SenviokRateLimitException>(() =>
            client.Sms.SendAsync("+123", "Acme", "Hi"));

        Assert.Equal((HttpStatusCode)429, ex.StatusCode);
        Assert.NotNull(ex.RetryAfter);
        Assert.Equal(TimeSpan.FromSeconds(60), ex.RetryAfter.Value);
        Assert.Equal("Too many requests. Please slow down.", ex.Message);
    }

    [Fact]
    public async Task Http500_ThrowsSenviokApiException()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.InternalServerError, "{\"message\": \"Internal server error\"}");
        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var ex = await Assert.ThrowsAsync<SenviokApiException>(() =>
            client.Emails.SendAsync(new SendEmailRequest { To = "x@y.com", Subject = "Hi" }));

        Assert.Equal(HttpStatusCode.InternalServerError, ex.StatusCode);
    }
}
