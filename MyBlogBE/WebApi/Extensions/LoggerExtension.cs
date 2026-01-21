using Application.Loggers;

namespace WebApi.Extensions;

public static class LoggerExtension
{
    /// <summary>
    /// Adds logger services from Application layer to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        services.AddSingleton<ApiLogger>();
        services.AddSingleton<TimerLogger>();
        services.AddSingleton<MyBlogLogger>();
        return services;
    }
}
