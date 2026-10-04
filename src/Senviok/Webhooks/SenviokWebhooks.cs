using System.Security.Cryptography;
using System.Text;

namespace Senviok.Webhooks;

/// <summary>
/// Helper utilities for verifying incoming webhook requests from Senviok.
/// </summary>
public static class SenviokWebhooks
{
    /// <summary>
    /// Verifies the cryptographic HMAC-SHA256 signature of a webhook payload.
    /// </summary>
    /// <param name="rawBody">The unparsed, exact string body received in the HTTP request.</param>
    /// <param name="signature">The signature header value (e.g. from 'X-Senviok-Signature').</param>
    /// <param name="secret">Your webhook endpoint secret.</param>
    /// <returns>True if the signature is valid; otherwise false.</returns>
    public static bool VerifySignature(string rawBody, string? signature, string? secret)
    {
        if (string.IsNullOrEmpty(rawBody) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secret))
        {
            return false;
        }

        var bodyBytes = Encoding.UTF8.GetBytes(rawBody);
        return VerifySignature(bodyBytes, signature!, secret!);
    }

    /// <summary>
    /// Verifies the cryptographic HMAC-SHA256 signature of a webhook payload.
    /// </summary>
    /// <param name="rawBody">The exact raw bytes received in the HTTP request.</param>
    /// <param name="signature">The signature header value (e.g. from 'X-Senviok-Signature').</param>
    /// <param name="secret">Your webhook endpoint secret.</param>
    /// <returns>True if the signature is valid; otherwise false.</returns>
    public static bool VerifySignature(byte[] rawBody, string? signature, string? secret)
    {
        if (rawBody == null || rawBody.Length == 0 || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secret))
        {
            return false;
        }

        try
        {
            var secretBytes = Encoding.UTF8.GetBytes(secret!);
            using var hmac = new HMACSHA256(secretBytes);
            var computedHash = hmac.ComputeHash(rawBody);
            var computedHex = ToHexString(computedHash);

            return FixedTimeEquals(computedHex, signature!.Trim());
        }
        catch
        {
            return false;
        }
    }

    private static string ToHexString(byte[] bytes)
    {
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        var bytesA = Encoding.UTF8.GetBytes(a.ToLowerInvariant());
        var bytesB = Encoding.UTF8.GetBytes(b.ToLowerInvariant());

#if NET8_0_OR_GREATER
        return CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
#else
        int result = 0;
        for (int i = 0; i < bytesA.Length; i++)
        {
            result |= bytesA[i] ^ bytesB[i];
        }
        return result == 0;
#endif
    }
}
