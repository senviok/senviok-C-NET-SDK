# Senviok .NET SDK

Official .NET / C# SDK for the **Senviok** communications infrastructure platform. Seamlessly integrate transactional **Email**, **SMS**, **WhatsApp**, **OTP**, **Webhooks**, **Templates**, **Audiences**, and **Domains** into your .NET applications.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET Multi-Target](https://img.shields.io/badge/.NET-netstandard2.0%20%7C%20net8.0%20%7C%20net10.0-purple.svg)](https://dotnet.microsoft.com/)

---

## Features

- **Multi-Targeting**: Supports modern `.NET 10`, `.NET 8 LTS`, and `.NET Standard 2.0` (compatible with .NET Framework 4.6.1+ and .NET Core 2.0+).
- **Multi-Channel Messaging**: First-class support for transactional Email, SMS, WhatsApp, and OTP.
- **Modern Async & HTTP-First**: Async-first API with `CancellationToken` support on every call.
- **Dependency Injection**: Seamless ASP.NET Core integration via `services.AddSenviok(...)`.
- **HMAC-SHA256 Verification**: Built-in, timing-safe webhook signature verification.
- **Typed Error Handling**: Specific exceptions for validation, authentication, rate limits (with `Retry-After`), and missing resources.

---

## Installation

Add the package via the .NET CLI:

```bash
dotnet add package Senviok
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package Senviok
```

---

## Quickstart

### 1. Direct Instantiation

```csharp
using Senviok;
using Senviok.Models.Emails;

// Initialize client with your API key
using var client = new SenviokClient("svk_live_xxxxxxxxxxxxxxxxxxxxxxxx");

// Dispatch a transactional email
var response = await client.Emails.SendAsync(new SendEmailRequest
{
    From = "Acme Team <onboarding@yourdomain.com>",
    To = "user@example.com",
    Subject = "Welcome to Acme!",
    Html = "<h1>Welcome aboard!</h1><p>We are glad to have you with us.</p>",
    Text = "Welcome aboard! We are glad to have you with us."
});

Console.WriteLine($"Email sent! Message ID: {response.Id}");
```

### 2. ASP.NET Core Dependency Injection

In your `Program.cs`:

```csharp
using Senviok.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Senviok with your API key
builder.Services.AddSenviok(builder.Configuration["Senviok:ApiKey"]!);

// Or with options:
builder.Services.AddSenviok(options =>
{
    options.ApiKey = builder.Configuration["Senviok:ApiKey"];
    options.BaseUrl = SenviokConstants.DefaultBaseUrl; // or SenviokConstants.LocalBaseUrl for local dev
    options.Timeout = TimeSpan.FromSeconds(30);
});
```

Inject `ISenviokClient` into your controllers, endpoints, or background workers:

```csharp
public class AuthController : ControllerBase
{
    private readonly ISenviokClient _senviok;

    public AuthController(ISenviokClient senviok)
    {
        _senviok = senviok;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Send OTP verification PIN
        var otp = await _senviok.Otp.SendAsync(new SendOtpRequest
        {
            To = request.PhoneNumber,
            From = "AcmeAuth",
            Channel = "SMS",
            Length = 6
        });

        return Ok(new { message = "Verification code dispatched", otp.ExpiresAt });
    }
}
```

---

## Usage Guide

### Email

Send emails to one or multiple recipients with attachments and templates:

```csharp
var email = await client.Emails.SendAsync(new SendEmailRequest
{
    From = "billing@acme.com",
    FromName = "Acme Invoicing",
    To = new[] { "alice@example.com", "bob@example.com" },
    Cc = "manager@example.com",
    Subject = "Your Monthly Invoice",
    Html = "<p>Please find attached your invoice.</p>",
    AddUnsubscribeFooter = true,
    Attachments = new List<EmailAttachment>
    {
        new()
        {
            Filename = "invoice.pdf",
            Content = Convert.ToBase64String(pdfBytes),
            ContentType = "application/pdf"
        }
    }
});
```

### SMS

```csharp
var sms = await client.Sms.SendAsync(
    to: "+1234567890",
    from: "AcmeAlert",
    text: "Your order #1042 has shipped!"
);
```

### WhatsApp

```csharp
var wa = await client.WhatsApp.SendAsync(new SendWhatsAppRequest
{
    To = "+1234567890",
    From = "AcmeWA",
    Text = "Hi Ada, your table reservation is confirmed!",
    TemplateId = "tmpl_reservation_update",
    Data = new Dictionary<string, object> { ["customer_name"] = "Ada" }
});
```

### OTP (One-Time Password)

Dispatch, verify, and resend secure verification codes:

```csharp
// 1. Send OTP
var otp = await client.Otp.SendAsync(new SendOtpRequest
{
    To = "+1234567890",
    From = "AcmeSecurity",
    Channel = "SMS", // or "Email"
    ExpiryMinutes = 5
});

// 2. Verify entered PIN
var verification = await client.Otp.VerifyAsync("+1234567890", "849201");
if (verification.Verified)
{
    Console.WriteLine("User verified successfully!");
}

// 3. Resend active PIN
await client.Otp.ResendAsync(new ResendOtpRequest
{
    To = "+1234567890",
    From = "AcmeSecurity"
});
```

### Webhook Verification

Verify incoming webhooks cryptographically using HMAC-SHA256:

```csharp
using Senviok.Webhooks;

[HttpPost("webhooks/senviok")]
public async Task<IActionResult> HandleWebhook()
{
    using var reader = new StreamReader(Request.Body);
    var rawBody = await reader.ReadToEndAsync();
    var signature = Request.Headers[SenviokConstants.WebhookSignatureHeader];
    var secret = Environment.GetEnvironmentVariable("SENVIOK_WEBHOOK_SECRET");

    if (!SenviokWebhooks.VerifySignature(rawBody, signature, secret))
    {
        return Unauthorized("Invalid webhook signature.");
    }

    // Process authenticated event
    return Ok();
}
```

### Managing Templates, Domains, Audiences & Keys

```csharp
// Templates
var template = await client.Templates.CreateAsync(new CreateTemplateRequest
{
    Name = "Welcome Email",
    Subject = "Welcome {{name}}!",
    HtmlContent = "<p>Hello {{name}}, welcome to our platform.</p>"
});

// Domains
var domain = await client.Domains.CreateAsync("mail.example.com");
var dkim = await client.Domains.GetDkimAsync(domain.Id);
var verifyResult = await client.Domains.VerifyAsync(domain.Id);

// Audiences & Contacts
var audience = await client.Audiences.CreateAsync("Beta Testers");
var contact = await client.Contacts.CreateAsync(audience.Id, new CreateContactRequest
{
    Email = "beta@example.com",
    FirstName = "Jane"
});

// Suppressions
await client.Suppressions.CreateAsync("unsubscribed@example.com", "unsubscribe");

// API Keys
var newKey = await client.ApiKeys.CreateAsync("Staging Server");
Console.WriteLine($"New Token: {newKey.Token}");
```

---

## Error Handling

The SDK provides typed exceptions inheriting from `SenviokException`:

| Exception | HTTP Status | Description |
| :--- | :--- | :--- |
| `SenviokValidationException` | `400` | Input or payload validation failure. Inspect `ErrorDetails.Errors` for field errors. |
| `SenviokAuthenticationException` | `401` / `403` | Invalid, missing, or unauthorized API key. |
| `SenviokNotFoundException` | `404` | Requested resource (e.g. domain, template) does not exist. |
| `SenviokRateLimitException` | `429` | Quota or rate limit exceeded. Check `RetryAfter` property for wait duration. |
| `SenviokApiException` | `5xx` / other | Server error or unexpected API response. |

```csharp
try
{
    await client.Emails.SendAsync(request);
}
catch (SenviokRateLimitException ex)
{
    Console.WriteLine($"Throttled. Retry in {ex.RetryAfter?.TotalSeconds} seconds.");
}
catch (SenviokValidationException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}
catch (SenviokAuthenticationException ex)
{
    Console.WriteLine($"Authentication failed: {ex.Message}");
}
catch (SenviokException ex)
{
    Console.WriteLine($"Senviok error ({ex.StatusCode}): {ex.Message}");
}
```

---

## Solution Structure

```
senviok-sdk-csharp/
├── src/
│   └── Senviok/
│       ├── Common/             # JsonDefaults, SenviokConstants, StringOrList
│       ├── Exceptions/         # SenviokException, Validation, Auth, RateLimit
│       ├── Extensions/         # ServiceCollectionExtensions (AddSenviok)
│       ├── Models/             # Request & Response DTOs
│       ├── Services/           # Emails, SMS, WhatsApp, OTP, Webhooks, etc.
│       ├── Webhooks/           # SenviokWebhooks (HMAC-SHA256 verifier)
│       ├── ISenviokClient.cs   # Public client contract
│       └── SenviokClient.cs    # Primary implementation
├── tests/
│   └── Senviok.Tests/          # 30 passing unit tests (xUnit)
└── samples/
    └── Senviok.Sample/         # Quickstart runnable console application
```

---

## License

This project is licensed under the [MIT License](LICENSE).

