// EntraUser.Infrastructure/Graph/GraphUserService.cs
namespace EntraUser.Infrastructure.Graph;

using EntraUser.Core.DTOs;
using EntraUser.Core.Interfaces;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Kiota.Abstractions.Authentication;

public class GraphUserService(GraphServiceClient appGraph)
    : IGraphUserService
{
    public async Task<UserProfileDto> GetMeAsync(
        string accessToken, CancellationToken ct = default)
    {
        var delegatedGraph = new GraphServiceClient(
            new BaseBearerTokenAuthenticationProvider(
                new StaticAccessTokenProvider(accessToken)));
        try
        {
            var me = await delegatedGraph.Me
                .GetAsync(req =>
                {
                    req.QueryParameters.Select =
                        ["id", "displayName", "givenName",
                         "surname", "userPrincipalName",
                         "passwordProfile"];
                }, cancellationToken: ct);

            var forceChange =
                me?.PasswordProfile?.ForceChangePasswordNextSignIn ?? false;

            System.Diagnostics.Debug.WriteLine(
                $"[GraphUserService] GET /me · " +
                $"UPN={me?.UserPrincipalName} · ForceChange={forceChange}");

            return new UserProfileDto(
                me!.Id!, me.UserPrincipalName ?? "",
                me.DisplayName ?? "", me.GivenName ?? "",
                me.Surname ?? "", forceChange);
        }
        catch (ODataError ex)
        {
            throw new InvalidOperationException(
                $"GET /me failed: {ex.Error?.Message}", ex);
        }
    }

    public async Task UpdatePasswordAsync(
        string userObjectId, string newPassword,
        CancellationToken ct = default)
    {
        try
        {
            await appGraph.Users[userObjectId]
                .PatchAsync(new User
                {
                    PasswordProfile = new PasswordProfile
                    {
                        Password                      = newPassword,
                        ForceChangePasswordNextSignIn = false
                    }
                }, cancellationToken: ct);

            System.Diagnostics.Debug.WriteLine(
                $"[GraphUserService] Password updated · ObjectId={userObjectId}");
        }
        catch (ODataError ex)
        {
            var code    = ex.Error?.Code    ?? "";
            var message = ex.Error?.Message ?? "";

            var friendly = (message.Contains("Insufficient privileges") ||
                            code == "Authorization_RequestDenied")
                ? "Password Administrator role is not assigned to the " +
                  "service principal. Assign it in Entra ID → " +
                  "Roles and administrators → Password Administrator."
                : $"Failed to update password: {message} [Code: {code}]";

            throw new InvalidOperationException(friendly, ex);
        }
    }
}

internal class StaticAccessTokenProvider(string token)
    : IAccessTokenProvider
{
    public Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken ct = default)
        => Task.FromResult(token);

    public AllowedHostsValidator AllowedHostsValidator { get; } =
        new(["graph.microsoft.com"]);
}
