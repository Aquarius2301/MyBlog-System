using Application.Services.Interfaces;
using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

public class FollowService : IFollowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private Guid AccountId => _jwtService.GetAccountInfo().Id;

    public FollowService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task FollowUserAsync(Guid followingId)
    {
        var existedFollow = await _unitOfWork
            .Follows.ReadOnly()
            .Where(f => f.AccountId == AccountId && f.FollowingId == followingId)
            .FirstOrDefaultAsync();

        if (existedFollow != null)
        {
            return; // Already following, do nothing
        }

        var follow = new Follow { AccountId = AccountId, FollowingId = followingId };
        _unitOfWork.Follows.Add(follow);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnfollowUserAsync(Guid followingId)
    {
        var existedFollow = await _unitOfWork
            .Follows.ReadOnly()
            .Where(f => f.AccountId == AccountId && f.FollowingId == followingId)
            .FirstOrDefaultAsync();

        if (existedFollow == null)
        {
            return; // Not following, do nothing
        }

        _unitOfWork.Follows.Remove(existedFollow);

        await _unitOfWork.SaveChangesAsync();
    }
}
