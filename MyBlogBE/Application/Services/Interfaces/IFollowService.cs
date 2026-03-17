using Application.Dtos;

public interface IFollowService
{
    Task<int> FollowUserAsync(Guid followingId);
    Task<int> UnfollowUserAsync(Guid followingId);
    Task<(List<AccountNameResponse>, DateTime?)> GetFollowersAsync(
        Guid userId,
        int pageSize,
        DateTime? cursor
    );
    // Task<IEnumerable<Guid>> GetFollowingAsync(Guid userId);
}
