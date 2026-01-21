using Application.Helpers;
using Application.Loggers;
using Application.Services.Interfaces;
using BusinessObject.Enums;
using DataAccess.Extensions;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Hosts;

/// <summary>
/// Background service that periodically cleans up user accounts.
/// </summary>
/// <remarks>
/// This service performs two main tasks every minute:
/// 1. Deletes unverified accounts that have expired email verification codes.
/// 2. Soft-deletes accounts that are scheduled for self-removal.
/// </remarks>
public class AccountCleanupHost : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(60);
    private MyBlogLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountCleanupHost"/> class.
    /// </summary>
    /// <param name="serviceProvider">
    /// The service provider used to create scopes for accessing application services such as <see cref="IUnitOfWork"/>.
    /// </param>
    public AccountCleanupHost(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = new MyBlogLogger("background.log");
    }

    /// <summary>
    /// Executes the background task repeatedly at a fixed interval until the service is stopped.
    /// </summary>
    /// <param name="stoppingToken">A cancellation token that signals when the service should stop.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.WhenAll([
                BackgroundAsync(CleanupExpiredAccountsAsync(), "CleanupExpiredAccountsAsync"),
                BackgroundAsync(SelfRemoveAccountsAsync(), "SelfRemoveAccountsAsync"),
                BackgroundAsync(RemovePicture(), "RemovePicture"),
            ]);

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task BackgroundAsync(Task<int> task, string taskName)
    {
        var startTime = DateTime.Now;

        var res = await task;

        await _logger.LogInfo(
            taskName,
            new { duration = DateTime.Now - startTime, message = $"Cleanup completed {res} items." }
        );
    }

    /// <summary>
    /// Deletes accounts that are inactive and whose email verification code has expired.
    /// </summary>
    /// <returns>A task representing the asynchronous cleanup operation.</returns>
    /// <remarks>
    /// Accounts that meet the criteria are permanently deleted from the database.
    /// Logs the number of deleted accounts to the console.
    /// </remarks>
    private async Task<int> CleanupExpiredAccountsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;
        var expiredAccounts = await unitOfWork
            .Accounts.GetQuery()
            .Where(a => a.Status == StatusType.Inactive.Code && a.EmailVerifiedCodeExpiry <= now)
            .ToListAsync();

        if (expiredAccounts.Any())
        {
            unitOfWork.Accounts.RemoveRange(expiredAccounts);
            await unitOfWork.SaveChangesAsync();
        }

        return expiredAccounts.Count;
    }

    /// <summary>
    /// Soft-deletes accounts that are scheduled for self-removal.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Sets <c>DeletedAt</c> to the current time and changes <c>Status</c> to <see cref="StatusType.Suspended"/>.
    /// Clears <c>SelfRemoveTime</c> after processing.
    /// Logs the number of accounts soft-deleted to the console.
    /// </remarks>
    private async Task<int> SelfRemoveAccountsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;
        var expiredAccounts = await unitOfWork
            .Accounts.GetQuery()
            .WhereSelfRemoveTimeIsLessThanNow()
            .ToListAsync();

        if (expiredAccounts.Any())
        {
            foreach (var account in expiredAccounts)
            {
                account.SelfRemoveTime = null;
                account.DeletedAt = now;
                account.Status = StatusType.Suspended.Code;

                await unitOfWork.SaveChangesAsync();
            }
        }

        return expiredAccounts.Count;
    }

    /// <summary>
    /// Removes pictures that are not associated with any account, post, or comment.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Removes pictures that are not associated with any account, post, or comment.
    /// Logs the number of pictures removed to the console.
    /// </remarks>
    private async Task<int> RemovePicture()
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var uploadService = scope.ServiceProvider.GetRequiredService<IUploadService>();
        var pictures = await unitOfWork.Pictures.GetQuery().WhereIsNotBeUsed().ToListAsync();

        Console.WriteLine($"Deleting pictures from imgbb... {pictures.Count}");

        await uploadService.DeleteImageAsync(pictures.Select(p => p.PublicId).ToList());

        unitOfWork.Pictures.RemoveRange(pictures);
        await unitOfWork.SaveChangesAsync();

        return pictures.Count;
    }
}
