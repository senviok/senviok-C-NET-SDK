namespace Senviok.Models.WhatsApp;

/// <summary>
/// Parameters for sending a WhatsApp message.
/// </summary>
public class SendWhatsAppRequest
{
    /// <summary>Recipient phone number in international E.164 format.</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>Registered WhatsApp sender ID or sender phone number.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>Text content of the WhatsApp message.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Optional pre-approved WhatsApp template ID for proactive outbound messaging.</summary>
    public string? TemplateId { get; set; }

    /// <summary>Optional dynamic template variables (e.g. {"1": "Value"} or {"customer_name": "Ada"}).</summary>
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>Optional registered WhatsApp device ID.</summary>
    public string? DeviceId { get; set; }
}

/// <summary>
/// Response returned after dispatching a WhatsApp message.
/// </summary>
public class SendWhatsAppResponse
{
    /// <summary>Unique ID of the message log.</summary>
    public string Id { get; set; } = string.Empty;
}
