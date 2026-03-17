using Application.Dtos;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;

public class FollowService : IFollowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private Guid AccountId => _jwtService.GetAccountInfo().Id;

    public FollowService(IUnitOfWork unitOfWork, IJwtService jwtService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<int> FollowUserAsync(Guid followingId)
    {
        var existedFollow = await _unitOfWork
            .Follows.ReadOnly()
            .Where(f => f.AccountId == AccountId && f.FollowingId == followingId)
            .FirstOrDefaultAsync();

        if (existedFollow != null)
        {
            return _unitOfWork.Follows.ReadOnly().Count(f => f.FollowingId == followingId); // Already following, do nothing
        }

        var follow = new Follow
        {
            AccountId = AccountId,
            FollowingId = followingId,
            CreatedAt = DateTime.UtcNow,
        };
        _unitOfWork.Follows.Add(follow);

        await _unitOfWork.SaveChangesAsync();

        return _unitOfWork.Follows.ReadOnly().Count(f => f.FollowingId == followingId);
    }

    public async Task<int> UnfollowUserAsync(Guid followingId)
    {
        var existedFollow = await _unitOfWork
            .Follows.ReadOnly()
            .Where(f => f.AccountId == AccountId && f.FollowingId == followingId)
            .FirstOrDefaultAsync();

        if (existedFollow == null)
        {
            return _unitOfWork.Follows.ReadOnly().Count(f => f.FollowingId == followingId); // Not following, do nothing
        }

        _unitOfWork.Follows.Remove(existedFollow);

        await _unitOfWork.SaveChangesAsync();

        return _unitOfWork.Follows.ReadOnly().Count(f => f.FollowingId == followingId);
    }

    public async Task<(List<AccountNameResponse>, DateTime?)> GetFollowersAsync(
        Guid userId,
        int pageSize,
        DateTime? cursor
    )
    {
        var followers = await _unitOfWork
            .Follows.ReadOnly()
            .Where(f => f.FollowingId == userId && (cursor == null || f.CreatedAt < cursor))
            .OrderByDescending(f => f.CreatedAt)
            .Take(pageSize + 1)
            .Select(x => new { Account = _mapper.Map<AccountNameResponse>(x.Account), x.CreatedAt })
            .ToListAsync();

        Console.WriteLine(
            $"Fetched {followers.Count} followers for user {userId} with cursor {cursor}"
        );

        var cursorDate =
            followers.Count > pageSize ? followers[pageSize - 1].CreatedAt : (DateTime?)null;

        var res = followers.Take(pageSize).Select(x => x.Account).ToList();

        return (res, cursorDate);
    }
}
