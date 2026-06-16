using System.Reflection;
using EntraUser.Core.Features.SetPin;
using EntraUser.Core.Services;
using EntraUser.Infrastructure;
using EntraUser.Infrastructure.Graph;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Radzen;


namespace EntraUser.MauiBlazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("appsettings.json"))
                ?? throw new InvalidOperationException("appsettings.json not found.");

            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
            builder.Services.AddSingleton<IConfiguration>(config);

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "entrauser.db");
            builder.Services.AddInfrastructure(config, dbPath);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<SetPinHandler>();
                cfg.RegisterServicesFromAssemblyContaining<GraphUserService>();
            });

            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddRadzenComponents();

            return builder.Build();
        }
    }
}
