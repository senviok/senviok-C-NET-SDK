using Senviok;
using Senviok.Models.Emails;
using Senviok.Models.Messages;
using Senviok.Models.Otp;
using Senviok.Models.Sms;
using Senviok.Models.WhatsApp;
using Senviok.Webhooks;

Console.WriteLine("=================================================");
Console.WriteLine("   Senviok .NET SDK - Quickstart Demonstration   ");
Console.WriteLine("=================================================");

var apiKey = Environment.GetEnvironmentVariable("SENVIOK_API_KEY") ?? "svk_test_demo_key";
var baseUrl = Environment.GetEnvironmentVariable("SENVIOK_BASE_URL") ?? SenviokConstants.LocalBaseUrl;

Console.WriteLine($"API Key:  {apiKey[..Math.Min(12, apiKey.Length)]}***");
Console.WriteLine($"Base URL: {baseUrl}\n");

using var client = new SenviokClient(apiKey, new SenviokClientOptions
{
    BaseUrl = baseUrl,
    Timeout = TimeSpan.FromSeconds(2)
});

// 1. Transactional Email
Console.WriteLine("1. Dispatching Transactional Email...");
try
{
    var emailResponse = await client.Emails.SendAsync(new SendEmailRequest
    {
        From = "onboarding@example.com",
        FromName = "Acme Team",
        To = "dev@example.com",
        Subject = "Welcome to Senviok via C# SDK!",
        Html = "<h1>Hello from C# / .NET SDK!</h1><p>This message was dispatched using the official Senviok .NET SDK.</p>",
        Text = "Hello from C# / .NET SDK! This message was dispatched using the official Senviok .NET SDK."
    });
    Console.WriteLine($"   [SUCCESS] Email queued! Message ID: {emailResponse.Id}");
}
catch (Exception ex)
{
    Console.WriteLine($"   [NOTE] {ex.Message}");
}

// 2. Transactional SMS
Console.WriteLine("\n2. Dispatching SMS Notification...");
try
{
    var smsResponse = await client.Sms.SendAsync(new SendSmsRequest
    {
        To = "+1234567890",
        From = "AcmeAlert",
        Text = "Your Senviok verification code is 492041."
    });
    Console.WriteLine($"   [SUCCESS] SMS sent! Message ID: {smsResponse.Id}");
}
catch (Exception ex)
{
    Console.WriteLine($"   [NOTE] {ex.Message}");
}

// 3. WhatsApp Messaging
Console.WriteLine("\n3. Dispatching WhatsApp Message...");
try
{
    var waResponse = await client.WhatsApp.SendAsync(new SendWhatsAppRequest
    {
        To = "+1234567890",
        From = "AcmeWA",
        Text = "Hello from Senviok WhatsApp API via C# SDK!"
    });
    Console.WriteLine($"   [SUCCESS] WhatsApp message sent! Message ID: {waResponse.Id}");
}
catch (Exception ex)
{
    Console.WriteLine($"   [NOTE] {ex.Message}");
}

// 4. OTP Dispatch & Verification
Console.WriteLine("\n4. Dispatching & Verifying One-Time Password (OTP)...");
try
{
    var otpResponse = await client.Otp.SendAsync(new SendOtpRequest
    {
        To = "+1234567890",
        From = "AcmeAuth",
        Length = 6,
        ExpiryMinutes = 5
    });
    Console.WriteLine($"   [SUCCESS] OTP dispatched! Message ID: {otpResponse.MessageId}, Expires At: {otpResponse.ExpiresAt:u}");
}
catch (Exception ex)
{
    Console.WriteLine($"   [NOTE] {ex.Message}");
}

// 5. Message Logs & Delivery History
Console.WriteLine("\n5. Querying Recent Message Logs...");
try
{
    var logs = await client.Messages.ListAsync(new MessageListOptions
    {
        Take = 5,
        SortOrder = "desc"
    });
    Console.WriteLine($"   [SUCCESS] Retrieved {logs.Count} log records.");
    foreach (var log in logs)
    {
        Console.WriteLine($"   - [{log.Channel.ToUpper()}] {log.Status} | To: {log.ToAddress} | Subject: {log.Subject}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"   [NOTE] {ex.Message}");
}

// 6. Webhook HMAC-SHA256 Signature Verification
Console.WriteLine("\n6. Webhook Cryptographic Signature Verification...");
var rawPayload = "{\"event\":\"email.delivered\",\"data\":{\"id\":\"msg_123\"}}";
var secret = "whsec_sample_secret_key";
using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret));
var validSig = BitConverter.ToString(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawPayload))).Replace("-", "").ToLowerInvariant();

var verified = SenviokWebhooks.VerifySignature(rawPayload, validSig, secret);
Console.WriteLine($"   [VERIFICATION] Signature matches payload: {verified}");

var tamperedVerified = SenviokWebhooks.VerifySignature("tampered content", validSig, secret);
Console.WriteLine($"   [SECURITY] Tampered payload rejected: {!tamperedVerified}");

Console.WriteLine("\nDemo run finished successfully.");
