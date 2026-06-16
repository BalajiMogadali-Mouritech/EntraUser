// EntraUser.Core/Interfaces/IPinService.cs
namespace EntraUser.Core.Interfaces;

public interface IPinService
{
    string HashPin(string pin);
    bool   VerifyPin(string pin, string storedHash);
}
