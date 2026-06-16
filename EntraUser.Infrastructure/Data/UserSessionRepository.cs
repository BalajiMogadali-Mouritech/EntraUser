// EntraUser.Infrastructure/Data/UserSessionRepository.cs
namespace EntraUser.Infrastructure.Data;

using EntraUser.Core.Interfaces;
using EntraUser.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class UserSessionRepository(AppDbContext db)
    : IUserSessionRepository
{
    public async Task<bool> TableExistsAsync(CancellationToken ct = default)
    {
        try   { await db.UserSessions.AnyAsync(ct); return true; }
        catch { return false; }
    }

    public async Task EnsureCreatedAsync(CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);
        System.Diagnostics.Debug.WriteLine("[UserSessionRepository] DB created.");
    }

    public async Task<UserSession?> GetByUpnAsync(
        string upn, CancellationToken ct = default)
        => await db.UserSessions
            .FirstOrDefaultAsync(x => x.UserUpn == upn, ct);

    public async Task UpsertAsync(
        UserSession session, CancellationToken ct = default)
    {
        var existing = await db.UserSessions
            .FirstOrDefaultAsync(x => x.UserUpn == session.UserUpn, ct);

        if (existing is null)
        {
            db.UserSessions.Add(session);
        }
        else
        {
            existing.DisplayName            = session.DisplayName;
            existing.ObjectId               = session.ObjectId;
            existing.PasswordChangeRequired = session.PasswordChangeRequired;
            existing.AccessToken            = session.AccessToken;
            existing.RefreshToken           = session.RefreshToken;
            existing.TokenExpiresAt         = session.TokenExpiresAt;
            existing.LastLoginAt            = session.LastLoginAt;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task SetPinHashAsync(
        string upn, string hash, CancellationToken ct = default)
    {
        var session = await db.UserSessions
            .FirstOrDefaultAsync(x => x.UserUpn == upn, ct)
            ?? throw new InvalidOperationException(
                $"Session not found for '{upn}'.");

        session.PinHash  = hash;
        session.HasPin   = true;
        session.PinSetAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateLastLoginAsync(
        string upn, CancellationToken ct = default)
    {
        var s = await db.UserSessions
            .FirstOrDefaultAsync(x => x.UserUpn == upn, ct);
        if (s is not null)
        {
            s.LastLoginAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task MarkPasswordChangedAsync(
        string upn, CancellationToken ct = default)
    {
        var s = await db.UserSessions
            .FirstOrDefaultAsync(x => x.UserUpn == upn, ct);
        if (s is not null)
        {
            s.PasswordChangeRequired = false;
            s.PasswordChangedAt      = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task UpdateTokensAsync(
        string upn, string accessToken, string refreshToken,
        DateTime expiresAt, CancellationToken ct = default)
    {
        var s = await db.UserSessions
            .FirstOrDefaultAsync(x => x.UserUpn == upn, ct);
        if (s is not null)
        {
            s.AccessToken    = accessToken;
            s.RefreshToken   = refreshToken;
            s.TokenExpiresAt = expiresAt;
            await db.SaveChangesAsync(ct);
        }
    }

    // Infrastructure/Data/UserSessionRepository.cs
    public async Task<List<UserSession>> GetPendingSyncAsync(
        CancellationToken ct = default)
        => await db.UserSessions
            .Where(x => x.HasPin && x.IsActive)
            .ToListAsync(ct);
}
