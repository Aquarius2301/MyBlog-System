using Application.Services.Implementations;
using Application.Services.Interfaces;
using DataAccess.Repositories;
using DataAccess.UnitOfWork;

namespace WebApi.Extensions;

public static class ApplicationExtension
{
    /// <summary>
    /// Add Services from Application layer
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IUploadService, UploadService>();

        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IPostService, PostService>();

        return services;
    }

    /// <summary>
    /// Add Repositories, Unit Of Work from Data Access layer
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
