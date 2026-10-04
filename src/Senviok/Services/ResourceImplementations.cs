using Senviok.Models.Audiences;
using Senviok.Models.ApiKeys;
using Senviok.Models.Contacts;
using Senviok.Models.Domains;
using Senviok.Models.Emails;
using Senviok.Models.Messages;
using Senviok.Models.Otp;
using Senviok.Models.Sms;
using Senviok.Models.Suppressions;
using Senviok.Models.Templates;
using Senviok.Models.Webhooks;
using Senviok.Models.WhatsApp;
using Senviok.Webhooks;

namespace Senviok.Services;

internal class EmailsResource : IEmailsResource
{
    private readonly SenviokClient _client;

    public EmailsResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<SendEmailResponse> SendAsync(SendEmailRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<SendEmailRequest, SendEmailResponse>(HttpMethod.Post, "emails", request, options, cancellationToken);
    }
}

internal class SmsResource : ISmsResource
{
    private readonly SenviokClient _client;

    public SmsResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<SendSmsResponse> SendAsync(SendSmsRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<SendSmsRequest, SendSmsResponse>(HttpMethod.Post, "sms", request, options, cancellationToken);
    }

    public Task<SendSmsResponse> SendAsync(string to, string from, string text, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SendAsync(new SendSmsRequest { To = to, From = from, Text = text }, options, cancellationToken);
    }
}

internal class WhatsAppResource : IWhatsAppResource
{
    private readonly SenviokClient _client;

    public WhatsAppResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<SendWhatsAppResponse> SendAsync(SendWhatsAppRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<SendWhatsAppRequest, SendWhatsAppResponse>(HttpMethod.Post, "whatsapp", request, options, cancellationToken);
    }

    public Task<SendWhatsAppResponse> SendAsync(string to, string from, string text, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SendAsync(new SendWhatsAppRequest { To = to, From = from, Text = text }, options, cancellationToken);
    }
}

internal class OtpResource : IOtpResource
{
    private readonly SenviokClient _client;

    public OtpResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<SendOtpResponse> SendAsync(SendOtpRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<SendOtpRequest, SendOtpResponse>(HttpMethod.Post, "otp/send", request, options, cancellationToken);
    }

    public Task<VerifyOtpResponse> VerifyAsync(VerifyOtpRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<VerifyOtpRequest, VerifyOtpResponse>(HttpMethod.Post, "otp/verify", request, options, cancellationToken);
    }

    public Task<VerifyOtpResponse> VerifyAsync(string to, string pin, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return VerifyAsync(new VerifyOtpRequest { To = to, Pin = pin }, options, cancellationToken);
    }

    public Task<ResendOtpResponse> ResendAsync(ResendOtpRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<ResendOtpRequest, ResendOtpResponse>(HttpMethod.Post, "otp/resend", request, options, cancellationToken);
    }
}

internal class MessagesResource : IMessagesResource
{
    private readonly SenviokClient _client;

    public MessagesResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<IReadOnlyList<MessageLog>> ListAsync(MessageListOptions? options = null, SenviokRequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
    {
        var query = options?.ToQueryParameters();
        return _client.SendListAsync<MessageLog>(HttpMethod.Get, "analytics/logs", query, requestOptions, cancellationToken);
    }
}

internal class WebhooksResource : IWebhooksResource
{
    private readonly SenviokClient _client;

    public WebhooksResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<Webhook> CreateAsync(CreateWebhookRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<CreateWebhookRequest, Webhook>(HttpMethod.Post, "webhooks", request, options, cancellationToken);
    }

    public Task<Webhook> CreateAsync(string url, IEnumerable<string> events, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync(new CreateWebhookRequest { Url = url, Events = events?.ToList() ?? new List<string>() }, options, cancellationToken);
    }

    public Task<IReadOnlyList<Webhook>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _client.SendListAsync<Webhook>(HttpMethod.Get, "webhooks", null, options, cancellationToken);
    }

    public Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Webhook ID is required.", nameof(id));
        return _client.SendNoContentAsync(HttpMethod.Delete, $"webhooks/{Uri.EscapeDataString(id)}", options, cancellationToken);
    }

    public Task<IReadOnlyList<WebhookDelivery>> GetDeliveriesAsync(string webhookId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(webhookId)) throw new ArgumentException("Webhook ID is required.", nameof(webhookId));
        return _client.SendListAsync<WebhookDelivery>(HttpMethod.Get, $"webhooks/{Uri.EscapeDataString(webhookId)}/deliveries", null, options, cancellationToken);
    }

    public bool VerifySignature(string rawBody, string? signature, string? secret)
    {
        return SenviokWebhooks.VerifySignature(rawBody, signature, secret);
    }

    public bool VerifySignature(byte[] rawBody, string? signature, string? secret)
    {
        return SenviokWebhooks.VerifySignature(rawBody, signature, secret);
    }
}

internal class TemplatesResource : ITemplatesResource
{
    private readonly SenviokClient _client;

    public TemplatesResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<Template> CreateAsync(CreateTemplateRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<CreateTemplateRequest, Template>(HttpMethod.Post, "templates", request, options, cancellationToken);
    }

    public Task<IReadOnlyList<Template>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _client.SendListAsync<Template>(HttpMethod.Get, "templates", null, options, cancellationToken);
    }

    public Task<Template> GetAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Template ID is required.", nameof(id));
        return _client.SendAsync<Template>(HttpMethod.Get, $"templates/{Uri.EscapeDataString(id)}", options, cancellationToken);
    }

    public Task<Template> UpdateAsync(string id, UpdateTemplateRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Template ID is required.", nameof(id));
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<UpdateTemplateRequest, Template>(HttpMethod.Put, $"templates/{Uri.EscapeDataString(id)}", request, options, cancellationToken);
    }

    public Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Template ID is required.", nameof(id));
        return _client.SendNoContentAsync(HttpMethod.Delete, $"templates/{Uri.EscapeDataString(id)}", options, cancellationToken);
    }
}

internal class DomainsResource : IDomainsResource
{
    private readonly SenviokClient _client;

    public DomainsResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<Domain> CreateAsync(CreateDomainRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<CreateDomainRequest, Domain>(HttpMethod.Post, "domains", request, options, cancellationToken);
    }

    public Task<Domain> CreateAsync(string name, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync(new CreateDomainRequest { Name = name }, options, cancellationToken);
    }

    public Task<IReadOnlyList<Domain>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _client.SendListAsync<Domain>(HttpMethod.Get, "domains", null, options, cancellationToken);
    }

    public Task<DkimResponse> GetDkimAsync(string domainId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(domainId)) throw new ArgumentException("Domain ID is required.", nameof(domainId));
        return _client.SendAsync<DkimResponse>(HttpMethod.Get, $"domains/{Uri.EscapeDataString(domainId)}/dkim", options, cancellationToken);
    }

    public Task<VerifyDomainResponse> VerifyAsync(string domainId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(domainId)) throw new ArgumentException("Domain ID is required.", nameof(domainId));
        return _client.SendAsync<object, VerifyDomainResponse>(HttpMethod.Post, $"domains/{Uri.EscapeDataString(domainId)}/verify", new { }, options, cancellationToken);
    }
}

internal class AudiencesResource : IAudiencesResource
{
    private readonly SenviokClient _client;

    public AudiencesResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<Audience> CreateAsync(CreateAudienceRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<CreateAudienceRequest, Audience>(HttpMethod.Post, "audiences", request, options, cancellationToken);
    }

    public Task<Audience> CreateAsync(string name, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync(new CreateAudienceRequest { Name = name }, options, cancellationToken);
    }

    public Task<IReadOnlyList<Audience>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _client.SendListAsync<Audience>(HttpMethod.Get, "audiences", null, options, cancellationToken);
    }

    public Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Audience ID is required.", nameof(id));
        return _client.SendNoContentAsync(HttpMethod.Delete, $"audiences/{Uri.EscapeDataString(id)}", options, cancellationToken);
    }
}

internal class ContactsResource : IContactsResource
{
    private readonly SenviokClient _client;

    public ContactsResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<Contact> CreateAsync(string audienceId, CreateContactRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(audienceId)) throw new ArgumentException("Audience ID is required.", nameof(audienceId));
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<CreateContactRequest, Contact>(HttpMethod.Post, $"audiences/{Uri.EscapeDataString(audienceId)}/contacts", request, options, cancellationToken);
    }

    public Task<IReadOnlyList<Contact>> ListAsync(string audienceId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(audienceId)) throw new ArgumentException("Audience ID is required.", nameof(audienceId));
        return _client.SendListAsync<Contact>(HttpMethod.Get, $"audiences/{Uri.EscapeDataString(audienceId)}/contacts", null, options, cancellationToken);
    }

    public Task DeleteAsync(string audienceId, string contactId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(audienceId)) throw new ArgumentException("Audience ID is required.", nameof(audienceId));
        if (string.IsNullOrWhiteSpace(contactId)) throw new ArgumentException("Contact ID is required.", nameof(contactId));
        return _client.SendNoContentAsync(HttpMethod.Delete, $"audiences/{Uri.EscapeDataString(audienceId)}/contacts/{Uri.EscapeDataString(contactId)}", options, cancellationToken);
    }
}

internal class SuppressionsResource : ISuppressionsResource
{
    private readonly SenviokClient _client;

    public SuppressionsResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<Suppression> CreateAsync(CreateSuppressionRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<CreateSuppressionRequest, Suppression>(HttpMethod.Post, "suppressions", request, options, cancellationToken);
    }

    public Task<Suppression> CreateAsync(string email, string? reason = null, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync(new CreateSuppressionRequest { Email = email, Reason = reason }, options, cancellationToken);
    }

    public Task<IReadOnlyList<Suppression>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _client.SendListAsync<Suppression>(HttpMethod.Get, "suppressions", null, options, cancellationToken);
    }

    public Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Suppression ID is required.", nameof(id));
        return _client.SendNoContentAsync(HttpMethod.Delete, $"suppressions/{Uri.EscapeDataString(id)}", options, cancellationToken);
    }
}

internal class ApiKeysResource : IApiKeysResource
{
    private readonly SenviokClient _client;

    public ApiKeysResource(SenviokClient client)
    {
        _client = client;
    }

    public Task<ApiKeyCreated> CreateAsync(CreateApiKeyRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<CreateApiKeyRequest, ApiKeyCreated>(HttpMethod.Post, "api-keys", request, options, cancellationToken);
    }

    public Task<ApiKeyCreated> CreateAsync(string name, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync(new CreateApiKeyRequest { Name = name }, options, cancellationToken);
    }

    public Task<IReadOnlyList<ApiKey>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        return _client.SendListAsync<ApiKey>(HttpMethod.Get, "api-keys", null, options, cancellationToken);
    }

    public Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("API Key ID is required.", nameof(id));
        return _client.SendNoContentAsync(HttpMethod.Delete, $"api-keys/{Uri.EscapeDataString(id)}", options, cancellationToken);
    }
}
