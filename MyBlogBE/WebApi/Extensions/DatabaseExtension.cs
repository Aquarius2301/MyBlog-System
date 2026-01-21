using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Extensions;

public static class DatabaseExtension
{
    /// <summary>
    /// Adds database services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<MyBlogContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );

        return services;
    }
}
