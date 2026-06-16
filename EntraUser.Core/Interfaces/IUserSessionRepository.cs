// EntraUser.Core/Interfaces/IUserSessionRepository.cs
namespace EntraUser.Core.Interfaces;

using EntraUser.Domain.Entities;

public interface IUserSessionRepository
{
    Task<bool>         TableExistsAsync(CancellationToken ct = default);
    Task               EnsureCreatedAsync(CancellationToken ct = default);
    Task<UserSession?> GetByUpnAsync(string upn, CancellationToken ct = default);
    Task               UpsertAsync(UserSession session, CancellationToken ct = default);
    Task               SetPinHashAsync(string upn, string hash, CancellationToken ct = default);
    Task               UpdateLastLoginAsync(string upn, CancellationToken ct = default);
    Task               MarkPasswordChangedAsync(string upn, CancellationToken ct = default);
    Task               UpdateTokensAsync(string upn, string accessToken,
                           string refreshToken, DateTime expiresAt,
                           CancellationToken ct = default);
    // Core/Interfaces/IUserSessionRepository.cs — add this method
    Task<List<UserSession>> GetPendingSyncAsync(
        CancellationToken ct = default);
}
