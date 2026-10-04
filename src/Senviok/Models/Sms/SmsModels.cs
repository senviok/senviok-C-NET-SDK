namespace Senviok.Models.Sms;

/// <summary>
/// Parameters for sending an SMS message.
/// </summary>
public class SendSmsRequest
{
    /// <summary>Recipient phone number in international E.164 format (e.g. "+1234567890").</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>Optional list of multiple recipient phone numbers for bulk delivery.</summary>
    public List<string>? Recipients { get; set; }

    /// <summary>Sender ID or registered phone number.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>Text content of the SMS message.</summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Response returned after dispatching an SMS message.
/// </summary>
public class SendSmsResponse
{
    /// <summary>Unique ID of the message log.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Number of message parts or recipients dispatched.</summary>
    public int Count { get; set; } = 1;
}
