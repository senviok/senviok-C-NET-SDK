namespace Senviok.Models.Contacts;

/// <summary>
/// An individual subscriber contact within an audience.
/// </summary>
public class Contact
{
    public string Id { get; set; } = string.Empty;
    public string AudienceId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool? Unsubscribed { get; set; }
    public string? CreatedAt { get; set; }
}

/// <summary>
/// Parameters for adding or updating a contact in an audience.
/// </summary>
public class CreateContactRequest
{
    /// <summary>Contact's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Optional first name.</summary>
    public string? FirstName { get; set; }

    /// <summary>Optional last name.</summary>
    public string? LastName { get; set; }

    /// <summary>Whether the contact is unsubscribed from this audience.</summary>
    public bool? Unsubscribed { get; set; }
}
