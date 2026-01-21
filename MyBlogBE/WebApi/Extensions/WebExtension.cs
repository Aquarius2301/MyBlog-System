using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace WebApi.Extensions;

public static class WebExtension
{
    /// <summary>
    /// Adds web-related services (CORS, Localization) to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var frontendUrl = configuration["BaseSettings:FrontendUrl"]!;
        var chromeExtensionUrl = configuration["BaseSettings:ChromeExtensionUrl"]!;

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy(
                "_myAllowSpecificOrigins",
                policy =>
                {
                    policy
                        .WithOrigins(frontendUrl, chromeExtensionUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });

        // Localization
        services.AddLocalization(options => options.ResourcesPath = "");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("vi"),
                new CultureInfo("ja"),
            };
            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new AcceptLanguageHeaderRequestCultureProvider(),
            };
        });

        return services;
    }
}
