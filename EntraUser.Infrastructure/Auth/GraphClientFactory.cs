// EntraUser.Infrastructure/Auth/GraphClientFactory.cs
namespace EntraUser.Infrastructure.Auth;

using Azure.Identity;
using EntraUser.Infrastructure.Config;
using Microsoft.Graph;

public static class GraphClientFactory
{
    public static GraphServiceClient Create(AzureAdOptions opts)
    {
        if (string.IsNullOrWhiteSpace(opts.TenantId))
            throw new InvalidOperationException("AzureAd:TenantId is missing.");
        if (string.IsNullOrWhiteSpace(opts.ClientId))
            throw new InvalidOperationException("AzureAd:ClientId is missing.");
        if (string.IsNullOrWhiteSpace(opts.ClientSecret))
            throw new InvalidOperationException("AzureAd:ClientSecret is missing.");

        var credential = new ClientSecretCredential(
            opts.TenantId, opts.ClientId, opts.ClientSecret,
            new ClientSecretCredentialOptions
            {
                TokenCachePersistenceOptions = null
            });

        return new GraphServiceClient(credential,
            ["https://graph.microsoft.com/.default"]);
    }
}
