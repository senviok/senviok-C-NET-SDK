using Senviok.Services;

namespace Senviok;

/// <summary>
/// The primary client interface for the Senviok API.
/// Provides access to transactional Email, SMS, WhatsApp, OTP, Webhooks, Templates, Audiences, and Domains.
/// </summary>
public interface ISenviokClient : IDisposable
{
    /// <summary>Operations for sending transactional and template emails.</summary>
    IEmailsResource Emails { get; }

    /// <summary>Operations for sending SMS messages.</summary>
    ISmsResource Sms { get; }

    /// <summary>Operations for sending WhatsApp messages.</summary>
    IWhatsAppResource WhatsApp { get; }

    /// <summary>Operations for dispatching and verifying One-Time Passwords (OTP).</summary>
    IOtpResource Otp { get; }

    /// <summary>Operations for querying delivery history and analytics logs.</summary>
    IMessagesResource Messages { get; }

    /// <summary>Operations for managing webhook endpoints and verifying signatures.</summary>
    IWebhooksResource Webhooks { get; }

    /// <summary>Operations for managing message and email templates.</summary>
    ITemplatesResource Templates { get; }

    /// <summary>Operations for managing sending domains and DNS verification.</summary>
    IDomainsResource Domains { get; }

    /// <summary>Operations for managing recipient audience groups.</summary>
    IAudiencesResource Audiences { get; }

    /// <summary>Operations for managing contacts within audiences.</summary>
    IContactsResource Contacts { get; }

    /// <summary>Operations for managing the workspace suppression list.</summary>
    ISuppressionsResource Suppressions { get; }

    /// <summary>Operations for managing workspace API keys.</summary>
    IApiKeysResource ApiKeys { get; }
}
