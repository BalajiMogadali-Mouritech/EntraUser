// EntraUser.Core/Interfaces/IMsalAuthService.cs
namespace EntraUser.Core.Interfaces;

using EntraUser.Core.DTOs;

public interface IMsalAuthService
{
    Task<MsalAuthResultDto> SignInInteractiveAsync(CancellationToken ct = default);
    Task<MsalAuthResultDto> SignInSilentAsync(string upn, CancellationToken ct = default);
    Task SignOutAsync(CancellationToken ct = default);
}
