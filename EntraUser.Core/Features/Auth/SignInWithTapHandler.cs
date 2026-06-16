// EntraUser.Core/Features/Auth/SignInWithTapHandler.cs
namespace EntraUser.Core.Features.Auth;

using EntraUser.Core.DTOs;
using EntraUser.Core.Interfaces;
using EntraUser.Domain.Entities;
using MediatR;

public class SignInWithTapHandler(
    IMsalAuthService       msalAuth,
    IGraphUserService      graphUser,
    IUserSessionRepository repo)
    : IRequestHandler<SignInWithTapCommand, MsalAuthResultDto>
{
    public async Task<MsalAuthResultDto> Handle(
        SignInWithTapCommand command, CancellationToken ct)
    {
        // ── Step 1: MSAL interactive sign-in ─────────────────────
        var auth = await msalAuth.SignInInteractiveAsync(ct);
        if (!auth.Success) return auth;

        // ── Step 2: GET /me — check forceChangePasswordNextSignIn ─
        UserProfileDto profile;
        try
        {
            profile = await graphUser.GetMeAsync(auth.AccessToken, ct);
        }
        catch (Exception ex)
        {
            return auth with
            {
                Success = false,
                Message = $"Failed to get user profile: {ex.Message}"
            };
        }

        // ── Step 3: upsert local session ──────────────────────────
        await repo.UpsertAsync(new UserSession
        {
            UserUpn                = auth.UserUpn,
            DisplayName            = auth.DisplayName,
            ObjectId               = auth.ObjectId,
            PasswordChangeRequired = profile.ForceChangePasswordNextSignIn,
            HasPin                 = false,
            AccessToken            = auth.AccessToken,
            RefreshToken           = auth.RefreshToken,
            TokenExpiresAt         = auth.ExpiresAt,
            LastLoginAt            = DateTime.UtcNow
        }, ct);

        System.Diagnostics.Debug.WriteLine(
            $"[SignInWithTapHandler] UPN={auth.UserUpn} " +
            $"ForceChange={profile.ForceChangePasswordNextSignIn}");

        return auth with
        {
            Message = profile.ForceChangePasswordNextSignIn
                ? "TAP verified. Password change required."
                : "TAP verified. Please set your PIN."
        };
    }
}
