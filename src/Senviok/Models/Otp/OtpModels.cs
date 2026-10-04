namespace Senviok.Models.Otp;

/// <summary>
/// Parameters for generating and dispatching a one-time verification PIN.
/// </summary>
public class SendOtpRequest
{
    /// <summary>Recipient phone number or email address.</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>Sender ID, phone number, or sender email address.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>Delivery channel ("SMS" or "Email"). Defaults to "SMS".</summary>
    public string Channel { get; set; } = "SMS";

    /// <summary>Optional message template format (e.g. "Your verification code is {pin}").</summary>
    public string? Template { get; set; }

    /// <summary>PIN code length (typically 4 or 6). Defaults to 6.</summary>
    public int Length { get; set; } = 6;

    /// <summary>PIN expiration in minutes. Defaults to 5.</summary>
    public int ExpiryMinutes { get; set; } = 5;
}

/// <summary>
/// Response returned after generating and dispatching an OTP.
/// </summary>
public class SendOtpResponse
{
    /// <summary>ID of the underlying notification message.</summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>The recipient address the OTP was dispatched to.</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the OTP expires.</summary>
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Parameters for verifying an OTP PIN provided by the user.
/// </summary>
public class VerifyOtpRequest
{
    /// <summary>Recipient phone number or email address.</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>The PIN code entered by the user.</summary>
    public string Pin { get; set; } = string.Empty;
}

/// <summary>
/// Response returned after verifying an OTP PIN.
/// </summary>
public class VerifyOtpResponse
{
    /// <summary>True if the PIN was successfully verified; otherwise false.</summary>
    public bool Verified { get; set; }

    /// <summary>Descriptive result message.</summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Parameters for resending an active OTP.
/// </summary>
public class ResendOtpRequest
{
    /// <summary>Recipient phone number or email address.</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>Sender ID, phone number, or sender email address.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>Delivery channel ("SMS" or "Email"). Defaults to "SMS".</summary>
    public string Channel { get; set; } = "SMS";

    /// <summary>Optional message template format.</summary>
    public string? Template { get; set; }

    /// <summary>PIN code length. Defaults to 6.</summary>
    public int Length { get; set; } = 6;

    /// <summary>PIN expiration in minutes. Defaults to 5.</summary>
    public int ExpiryMinutes { get; set; } = 5;
}

/// <summary>
/// Response returned after resending an OTP.
/// </summary>
public class ResendOtpResponse
{
    /// <summary>ID of the underlying notification message.</summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>The recipient address.</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>True if the active PIN was resent.</summary>
    public bool Resent { get; set; }

    /// <summary>UTC timestamp when the OTP expires.</summary>
    public DateTime ExpiresAt { get; set; }
}
