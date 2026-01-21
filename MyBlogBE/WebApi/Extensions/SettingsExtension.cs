using Application.Settings;

namespace WebApi.Extensions;

public static class SettingsExtension
{
    /// <summary>
    /// Adds settings from appsettings.json to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddSettingsServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<BaseSettings>(configuration.GetSection("BaseSettings"));

        services.Configure<EmailSettings>(configuration.GetSection("BaseSettings:EmailSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("BaseSettings:JwtSettings"));
        services.Configure<UploadSettings>(configuration.GetSection("BaseSettings:UploadSettings"));

        return services;
    }
}
