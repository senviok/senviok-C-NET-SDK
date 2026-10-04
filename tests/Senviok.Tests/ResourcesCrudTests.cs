using System.Net;
using Senviok.Models.Audiences;
using Senviok.Models.ApiKeys;
using Senviok.Models.Contacts;
using Senviok.Models.Domains;
using Senviok.Models.Messages;
using Senviok.Models.Suppressions;
using Senviok.Models.Templates;

namespace Senviok.Tests;

public class ResourcesCrudTests
{
    [Fact]
    public async Task Templates_CrudOperationsWorkAsExpected()
    {
        var handler = new TestHttpMessageHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Post && path.EndsWith("/v1/templates"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\": \"tmpl_1\", \"name\": \"Welcome Template\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            if (req.Method == HttpMethod.Get && path.EndsWith("/v1/templates/tmpl_1"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\": \"tmpl_1\", \"name\": \"Welcome Template\", \"subject\": \"Hello!\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            if (req.Method == HttpMethod.Delete && path.EndsWith("/v1/templates/tmpl_1"))
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var created = await client.Templates.CreateAsync(new CreateTemplateRequest
        {
            Name = "Welcome Template",
            Subject = "Welcome!",
            HtmlContent = "<p>Hi {{name}}</p>"
        });
        Assert.Equal("tmpl_1", created.Id);

        var retrieved = await client.Templates.GetAsync("tmpl_1");
        Assert.Equal("tmpl_1", retrieved.Id);
        Assert.Equal("Hello!", retrieved.Subject);

        await client.Templates.DeleteAsync("tmpl_1");
    }

    [Fact]
    public async Task Domains_GetDkimAndVerifyWork()
    {
        var handler = new TestHttpMessageHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/v1/domains/dom_1/dkim"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"status\": \"Pending\", \"tokens\": [{\"name\": \"s1._domainkey.example.com\", \"value\": \"p=MIGf...\", \"type\": \"CNAME\"}]}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            if (path.EndsWith("/v1/domains/dom_1/verify"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"status\": \"Verified\", \"message\": \"Domain DNS confirmed.\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var dkim = await client.Domains.GetDkimAsync("dom_1");
        Assert.Equal("Pending", dkim.Status);
        Assert.Single(dkim.Tokens);
        Assert.Equal("s1._domainkey.example.com", dkim.Tokens[0].Name);

        var verify = await client.Domains.VerifyAsync("dom_1");
        Assert.Equal("Verified", verify.Status);
    }

    [Fact]
    public async Task AudiencesAndContacts_OperationsWork()
    {
        var handler = new TestHttpMessageHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Post && path.EndsWith("/v1/audiences"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\": \"aud_1\", \"name\": \"Beta Testers\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            if (req.Method == HttpMethod.Post && path.EndsWith("/v1/audiences/aud_1/contacts"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\": \"cnt_1\", \"audienceId\": \"aud_1\", \"email\": \"user@example.com\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            if (req.Method == HttpMethod.Delete)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var aud = await client.Audiences.CreateAsync("Beta Testers");
        Assert.Equal("aud_1", aud.Id);

        var contact = await client.Contacts.CreateAsync("aud_1", new CreateContactRequest
        {
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe"
        });
        Assert.Equal("cnt_1", contact.Id);
        Assert.Equal("user@example.com", contact.Email);

        await client.Contacts.DeleteAsync("aud_1", "cnt_1");
        await client.Audiences.DeleteAsync("aud_1");
    }

    [Fact]
    public async Task SuppressionsAndApiKeys_OperationsWork()
    {
        var handler = new TestHttpMessageHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Post && path.EndsWith("/v1/suppressions"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\": \"sup_1\", \"email\": \"unsub@example.com\", \"reason\": \"unsubscribe\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            if (req.Method == HttpMethod.Post && path.EndsWith("/v1/api-keys"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"id\": \"key_1\", \"name\": \"CI Bot\", \"token\": \"svk_live_secret123\"}", System.Text.Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var suppression = await client.Suppressions.CreateAsync("unsub@example.com", "unsubscribe");
        Assert.Equal("sup_1", suppression.Id);
        Assert.Equal("unsub@example.com", suppression.Email);

        var key = await client.ApiKeys.CreateAsync("CI Bot");
        Assert.Equal("key_1", key.Id);
        Assert.Equal("svk_live_secret123", key.Token);
    }

    [Fact]
    public async Task Messages_ListFormatsQueryParamsCorrectly()
    {
        HttpRequestMessage? capturedRequest = null;
        var handler = new TestHttpMessageHandler(req =>
        {
            capturedRequest = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"logs\": [{\"id\": \"msg_log_1\", \"channel\": \"email\", \"status\": \"delivered\"}]}", System.Text.Encoding.UTF8, "application/json")
            };
        });

        using var client = new SenviokClient("svk_test", new SenviokClientOptions { HttpMessageHandler = handler });

        var logs = await client.Messages.ListAsync(new MessageListOptions
        {
            Skip = 10,
            Take = 50,
            Channel = "email",
            Status = "delivered"
        });

        Assert.Single(logs);
        Assert.Equal("msg_log_1", logs[0].Id);

        Assert.NotNull(capturedRequest);
        var query = capturedRequest!.RequestUri!.Query;
        Assert.Contains("skip=10", query);
        Assert.Contains("take=50", query);
        Assert.Contains("channel=email", query);
        Assert.Contains("status=delivered", query);
    }
}
