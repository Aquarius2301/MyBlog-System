public interface IFollowService
{
    Task FollowUserAsync(Guid followingId);
    Task UnfollowUserAsync(Guid followingId);
    // Task<IEnumerable<Guid>> GetFollowersAsync(Guid userId);
    // Task<IEnumerable<Guid>> GetFollowingAsync(Guid userId);
}
