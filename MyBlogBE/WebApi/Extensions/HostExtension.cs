using WebApi.Hosts;

namespace WebApi.Extensions;

public static class HostExtension
{
    /// <summary>
    /// Adds hosted services from WebApi to the service collection.
    /// </summary>
    /// <param name="services"></param>
    public static void AddHostServices(this IServiceCollection services)
    {
        services.AddHostedService<AccountCleanupHost>();
    }
}
