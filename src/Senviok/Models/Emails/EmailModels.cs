using System.Text.Json.Serialization;
using Senviok.Common;

namespace Senviok.Models.Emails;

/// <summary>
/// Parameters for sending a transactional or template-based email.
/// </summary>
public class SendEmailRequest
{
    /// <summary>The sender address (e.g. "onboarding@yourdomain.com" or "Acme &lt;onboarding@yourdomain.com&gt;").</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>Optional display name of the sender.</summary>
    public string? FromName { get; set; }

    /// <summary>One or more recipient email addresses.</summary>
    public StringOrList To { get; set; } = new();

    /// <summary>The subject line of the email.</summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>HTML formatted email body.</summary>
    public string? Html { get; set; }

    /// <summary>Plain-text email body fallback.</summary>
    public string? Text { get; set; }

    /// <summary>Optional Carbon Copy (CC) recipients.</summary>
    public StringOrList? Cc { get; set; }

    /// <summary>Optional Blind Carbon Copy (BCC) recipients.</summary>
    public StringOrList? Bcc { get; set; }

    /// <summary>Optional Reply-To address(es).</summary>
    public StringOrList? ReplyTo { get; set; }

    /// <summary>Optional ID of a saved template in Senviok to render.</summary>
    public string? TemplateId { get; set; }

    /// <summary>Optional dictionary of key-value pairs to populate variables in the template.</summary>
    public Dictionary<string, string>? TemplateData { get; set; }

    /// <summary>Whether to append a compliant 1-click unsubscribe footer to the email.</summary>
    public bool? AddUnsubscribeFooter { get; set; }

    /// <summary>Whether to include RFC 8058 List-Unsubscribe headers.</summary>
    public bool? AddListUnsubscribeHeader { get; set; }

    /// <summary>Optional list of email file attachments.</summary>
    public List<EmailAttachment>? Attachments { get; set; }
}

/// <summary>
/// Represents a file attachment in an outgoing email.
/// </summary>
public class EmailAttachment
{
    /// <summary>The name of the attached file (e.g. "invoice.pdf").</summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>Base64-encoded file content.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>MIME content type (e.g. "application/pdf").</summary>
    public string? ContentType { get; set; }
}

/// <summary>
/// Response returned after successfully queuing or dispatching an email.
/// </summary>
public class SendEmailResponse
{
    /// <summary>Unique ID of the dispatched message.</summary>
    public string Id { get; set; } = string.Empty;
}
