namespace Senviok.Models.Messages;

/// <summary>
/// Detailed log record of a sent or queued message.
/// </summary>
public class MessageLog
{
    public string Id { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ToAddress { get; set; }
    public string? FromAddress { get; set; }
    public string? Subject { get; set; }
    public string? HtmlBody { get; set; }
    public string? TextBody { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? CcAddress { get; set; }
    public string? BccAddress { get; set; }
    public string? ReplyToAddress { get; set; }
    public string? CreatedAt { get; set; }
}

/// <summary>
/// Query filters for listing message history and analytics logs.
/// </summary>
public class MessageListOptions
{
    /// <summary>Number of records to skip for pagination. Defaults to 0.</summary>
    public int Skip { get; set; } = 0;

    /// <summary>Number of records to retrieve (page size). Defaults to 20.</summary>
    public int Take { get; set; } = 20;

    /// <summary>Sort order ("asc" or "desc"). Defaults to "desc".</summary>
    public string SortOrder { get; set; } = "desc";

    /// <summary>Filter by communication channel (e.g. "email", "sms", "whatsapp").</summary>
    public string? Channel { get; set; }

    /// <summary>Filter by message status (e.g. "queued", "delivered", "bounced", "failed").</summary>
    public string? Status { get; set; }

    /// <summary>Filter by recipient address or phone number.</summary>
    public string? ToAddress { get; set; }

    /// <summary>Filter by sender address or sender ID.</summary>
    public string? FromAddress { get; set; }

    /// <summary>Filter by subject line substring.</summary>
    public string? Subject { get; set; }

    /// <summary>Filter messages created on or after this ISO-8601 date.</summary>
    public string? StartDate { get; set; }

    /// <summary>Filter messages created on or before this ISO-8601 date.</summary>
    public string? EndDate { get; set; }

    public Dictionary<string, string> ToQueryParameters()
    {
        var dict = new Dictionary<string, string>
        {
            ["skip"] = Skip.ToString(),
            ["take"] = Take.ToString(),
            ["sortOrder"] = SortOrder
        };

        if (!string.IsNullOrWhiteSpace(Channel)) dict["channel"] = Channel!;
        if (!string.IsNullOrWhiteSpace(Status)) dict["status"] = Status!;
        if (!string.IsNullOrWhiteSpace(ToAddress)) dict["toAddress"] = ToAddress!;
        if (!string.IsNullOrWhiteSpace(FromAddress)) dict["fromAddress"] = FromAddress!;
        if (!string.IsNullOrWhiteSpace(Subject)) dict["subject"] = Subject!;
        if (!string.IsNullOrWhiteSpace(StartDate)) dict["startDate"] = StartDate!;
        if (!string.IsNullOrWhiteSpace(EndDate)) dict["endDate"] = EndDate!;

        return dict;
    }
}
