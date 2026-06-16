// EntraUser.Core/Interfaces/IGraphUserService.cs
namespace EntraUser.Core.Interfaces;

using EntraUser.Core.DTOs;

public interface IGraphUserService
{
    Task<UserProfileDto> GetMeAsync(string accessToken, CancellationToken ct = default);
    Task UpdatePasswordAsync(string userObjectId, string newPassword, CancellationToken ct = default);
}
