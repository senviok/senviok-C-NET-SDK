namespace Senviok.Models.Webhooks;

/// <summary>
/// A registered webhook subscription.
/// </summary>
public class Webhook
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public string? CreatedAt { get; set; }
}

/// <summary>
/// Parameters for creating a new webhook endpoint.
/// </summary>
public class CreateWebhookRequest
{
    /// <summary>The HTTPS destination URL where event payloads will be posted.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>The events to subscribe to (e.g. ["email.delivered", "email.bounced", "sms.delivered"]).</summary>
    public List<string> Events { get; set; } = new();
}

/// <summary>
/// Delivery audit log entry for a dispatched webhook event.
/// </summary>
public class WebhookDelivery
{
    public string Id { get; set; } = string.Empty;
    public string WebhookId { get; set; } = string.Empty;
    public string? EventId { get; set; }
    public string? EventType { get; set; }
    public int Attempt { get; set; }
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CreatedAt { get; set; }
}
