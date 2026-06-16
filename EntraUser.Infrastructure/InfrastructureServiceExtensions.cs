// EntraUser.Infrastructure/InfrastructureServiceExtensions.cs
namespace EntraUser.Infrastructure;

using EntraUser.Core.Interfaces;
using EntraUser.Infrastructure.Auth;
using EntraUser.Infrastructure.Config;
using EntraUser.Infrastructure.Data;
using EntraUser.Infrastructure.Graph;
using EntraUser.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration,
        string                  dbPath)
    {
        services
            .Configure<AzureAdOptions>(
                configuration.GetSection(AzureAdOptions.SectionName));

        services
            .AddOptions<AzureAdOptions>()
            .Bind(configuration.GetSection(AzureAdOptions.SectionName))
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.TenantId)    &&
                !string.IsNullOrWhiteSpace(o.ClientId)    &&
                !string.IsNullOrWhiteSpace(o.ClientSecret),
                "AzureAd:TenantId, ClientId and ClientSecret are required.")
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite($"Data Source={dbPath}"));

        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<AzureAdOptions>>().Value;
            return GraphClientFactory.Create(opts);
        });

        services.AddSingleton<IMsalAuthService, MsalAuthService>();
        services.AddScoped<IGraphUserService,   GraphUserService>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddSingleton<IPinService,      PinService>();

        return services;
    }
}
