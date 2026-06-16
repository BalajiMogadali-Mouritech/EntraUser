// EntraUser.Infrastructure/Services/PinService.cs
namespace EntraUser.Infrastructure.Services;

using EntraUser.Core.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

public class PinService : IPinService
{
    public string HashPin(string pin)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = KeyDerivation.Pbkdf2(
            pin, salt, KeyDerivationPrf.HMACSHA256, 100_000, 32);
        var combined = new byte[16 + 32];
        salt.CopyTo(combined, 0);
        hash.CopyTo(combined, 16);
        return Convert.ToBase64String(combined);
    }

    public bool VerifyPin(string pin, string storedHash)
    {
        byte[] combined;
        try   { combined = Convert.FromBase64String(storedHash); }
        catch { return false; }
        if (combined.Length < 48) return false;
        var salt     = combined[..16];
        var expected = combined[16..];
        var actual   = KeyDerivation.Pbkdf2(
            pin, salt, KeyDerivationPrf.HMACSHA256, 100_000, 32);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
