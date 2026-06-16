// EntraUser.Infrastructure/Config/AzureAdOptions.cs
namespace EntraUser.Infrastructure.Config;

public class AzureAdOptions
{
    public const string SectionName = "AzureAd";

    public string TenantId          { get; set; } = "";
    public string ClientId          { get; set; } = "";
    public string ClientSecret      { get; set; } = "";
    public string AppObjectId       { get; set; } = "";
    public string ServicePrincipalId{ get; set; } = "";
    public string TenantDomain      { get; set; } = "";
    public string RedirectUri       { get; set; } = "http://localhost";
}
