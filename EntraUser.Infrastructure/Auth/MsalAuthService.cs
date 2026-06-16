// EntraUser.Infrastructure/Auth/MsalAuthService.cs
namespace EntraUser.Infrastructure.Auth;

using EntraUser.Core.DTOs;
using EntraUser.Core.Interfaces;
using EntraUser.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

public class MsalAuthService : IMsalAuthService
{
    private readonly IPublicClientApplication _msal;

    private static readonly string[] Scopes =
        ["User.Read", "openid", "profile", "offline_access"];

    public MsalAuthService(IOptions<AzureAdOptions> opts)
    {
        var o = opts.Value;

        // ── Use http://localhost for Windows/Desktop ───────────────
        // Use custom scheme for Android/iOS
#if WINDOWS || MACCATALYST
        var redirectUri = "http://localhost";
#elif ANDROID
        var redirectUri = $"msal{o.ClientId}://auth";
#elif IOS
        var redirectUri = $"msal{o.ClientId}://auth";
#else
        var redirectUri = "http://localhost";
#endif

        _msal = PublicClientApplicationBuilder
            .Create(o.ClientId)
            .WithAuthority(
                $"https://login.microsoftonline.com/{o.TenantId}")
            .WithRedirectUri(redirectUri)
            .Build();

        System.Diagnostics.Debug.WriteLine(
            $"[MsalAuthService] RedirectUri={redirectUri}");
    }

    public async Task<MsalAuthResultDto> SignInInteractiveAsync(
        CancellationToken ct = default)
    {
        try
        {
            var result = await _msal
                .AcquireTokenInteractive(Scopes)
                .WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync(ct);

            System.Diagnostics.Debug.WriteLine(
                $"[MsalAuthService] Signed in · " +
                $"UPN={result.Account.Username}");

            return new MsalAuthResultDto(
                Success: true,
                UserUpn: result.Account.Username,
                DisplayName: result.ClaimsPrincipal?
                                  .FindFirst("name")?.Value
                              ?? result.Account.Username,
                ObjectId: result.ClaimsPrincipal?
                                  .FindFirst("oid")?.Value ?? "",
                AccessToken: result.AccessToken,
                RefreshToken: "",
                ExpiresAt: result.ExpiresOn.UtcDateTime,
                Message: "Signed in successfully.");
        }
        catch (MsalClientException ex)
            when (ex.ErrorCode == "authentication_canceled")
        {
            return Fail("Sign-in was cancelled.", ex.ErrorCode);
        }
        catch (MsalException ex)
        {
            return Fail($"MSAL error: {ex.Message}", ex.ErrorCode);
        }
        catch (Exception ex)
        {
            return Fail($"Sign-in failed: {ex.Message}");
        }
    }

    public async Task<MsalAuthResultDto> SignInSilentAsync(
        string upn, CancellationToken ct = default)
    {
        try
        {
            var accounts = await _msal.GetAccountsAsync();
            var account = accounts.FirstOrDefault(a =>
                a.Username.Equals(upn, StringComparison.OrdinalIgnoreCase));

            if (account is null)
                return Fail("No cached account. Please sign in again.");

            var result = await _msal
                .AcquireTokenSilent(Scopes, account)
                .ExecuteAsync(ct);

            return new MsalAuthResultDto(
                true,
                result.Account.Username,
                result.ClaimsPrincipal?.FindFirst("name")?.Value ?? upn,
                result.ClaimsPrincipal?.FindFirst("oid")?.Value ?? "",
                result.AccessToken, "",
                result.ExpiresOn.UtcDateTime,
                "Token refreshed.");
        }
        catch (MsalUiRequiredException)
        {
            return Fail("Session expired. Please sign in again.");
        }
        catch (MsalException ex)
        {
            return Fail($"Silent refresh failed: {ex.Message}", ex.ErrorCode);
        }
    }

    public async Task SignOutAsync(CancellationToken ct = default)
    {
        var accounts = await _msal.GetAccountsAsync();
        foreach (var account in accounts)
            await _msal.RemoveAsync(account);

        System.Diagnostics.Debug.WriteLine("[MsalAuthService] Signed out");
    }

    private static MsalAuthResultDto Fail(
        string message, string? code = null) =>
        new(false, "", "", "", "", "", DateTime.MinValue, message);
}