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

namespace Senviok.Services;

public interface IEmailsResource
{
    Task<SendEmailResponse> SendAsync(SendEmailRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface ISmsResource
{
    Task<SendSmsResponse> SendAsync(SendSmsRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<SendSmsResponse> SendAsync(string to, string from, string text, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface IWhatsAppResource
{
    Task<SendWhatsAppResponse> SendAsync(SendWhatsAppRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<SendWhatsAppResponse> SendAsync(string to, string from, string text, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface IOtpResource
{
    Task<SendOtpResponse> SendAsync(SendOtpRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<VerifyOtpResponse> VerifyAsync(VerifyOtpRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<VerifyOtpResponse> VerifyAsync(string to, string pin, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<ResendOtpResponse> ResendAsync(ResendOtpRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface IMessagesResource
{
    Task<IReadOnlyList<MessageLog>> ListAsync(MessageListOptions? options = null, SenviokRequestOptions? requestOptions = null, CancellationToken cancellationToken = default);
}

public interface IWebhooksResource
{
    Task<Webhook> CreateAsync(CreateWebhookRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<Webhook> CreateAsync(string url, IEnumerable<string> events, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Webhook>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WebhookDelivery>> GetDeliveriesAsync(string webhookId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    bool VerifySignature(string rawBody, string? signature, string? secret);
    bool VerifySignature(byte[] rawBody, string? signature, string? secret);
}

public interface ITemplatesResource
{
    Task<Template> CreateAsync(CreateTemplateRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Template>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<Template> GetAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<Template> UpdateAsync(string id, UpdateTemplateRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface IDomainsResource
{
    Task<Domain> CreateAsync(CreateDomainRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<Domain> CreateAsync(string name, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Domain>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<DkimResponse> GetDkimAsync(string domainId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<VerifyDomainResponse> VerifyAsync(string domainId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface IAudiencesResource
{
    Task<Audience> CreateAsync(CreateAudienceRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<Audience> CreateAsync(string name, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Audience>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface IContactsResource
{
    Task<Contact> CreateAsync(string audienceId, CreateContactRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Contact>> ListAsync(string audienceId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(string audienceId, string contactId, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface ISuppressionsResource
{
    Task<Suppression> CreateAsync(CreateSuppressionRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<Suppression> CreateAsync(string email, string? reason = null, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Suppression>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}

public interface IApiKeysResource
{
    Task<ApiKeyCreated> CreateAsync(CreateApiKeyRequest request, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<ApiKeyCreated> CreateAsync(string name, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApiKey>> ListAsync(SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, SenviokRequestOptions? options = null, CancellationToken cancellationToken = default);
}
