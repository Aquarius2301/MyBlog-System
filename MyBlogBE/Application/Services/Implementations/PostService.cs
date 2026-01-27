using Application.Dtos;
using Application.Exceptions;
using Application.Helpers;
using Application.Services.Interfaces;
using Application.Settings;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessObject.Entities;
using DataAccess.Extensions;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseSettings _settings;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private Guid AccountId => _jwtService.GetAccountInfo().Id;

    public PostService(
        IUnitOfWork unitOfWork,
        IOptions<BaseSettings> options,
        IMapper mapper,
        IJwtService jwtService
    )
    {
        _unitOfWork = unitOfWork;
        _settings = options.Value;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public async Task<(List<GetPostsResponse>, DateTime?)> GetPostsListAsync(
        DateTime? cursor,
        int pageSize
    )
    {
        var baseQuery = _unitOfWork
            .Posts.ReadOnly()
            .WhereDeletedIsNull()
            .WhereIf(cursor.HasValue, q => q.WhereCursorLessThan(cursor!.Value));

        var postsQuery = baseQuery.OrderByDescending(p => p.CreatedAt).Take(pageSize + 1);

        var posts = await postsQuery
            // .Select(p => new GetPostsResponse
            // {
            //     Id = p.Id,
            //     Link = p.Link,
            //     Content = p.Content,
            //     CreatedAt = p.CreatedAt,
            //     UpdatedAt = p.UpdatedAt,

            //     IsOwner = p.AccountId == accountId,

            //     LikeCount = p.PostLikes.Count(),
            //     CommentCount = p.Comments.Count(),

            //     IsLiked = p.PostLikes.Any(l => l.AccountId == accountId),

            //     Author = new AccountNameResponse
            //     {
            //         Id = p.Account.Id,
            //         Username = p.Account.Username,
            //         DisplayName = p.Account.DisplayName,
            //         Avatar = p.Account.Picture != null ? p.Account.Picture.Link : "",
            //         IsFollowing = p.Account.Followers.Any(f =>
            //             f.AccountId == accountId && f.FollowingId == p.AccountId
            //         ),
            //         CreatedAt = p.Account.CreatedAt,
            //     },

            //     PostPictures = p.Pictures.Select(pic => pic.Link).ToList(),

            //     LatestComment = p
            //         .Comments.Where(c => c.ParentCommentId == null)
            //         .OrderByDescending(c => c.CreatedAt)
            //         .Select(c => new PostLatestComment
            //         {
            //             Content = c.Content,
            //             CreatedAt = c.CreatedAt,
            //             Commenter = new AccountNameResponse
            //             {
            //                 Id = c.Account.Id,
            //                 Username = c.Account.Username,
            //                 DisplayName = c.Account.DisplayName,
            //                 Avatar = c.Account.Picture != null ? c.Account.Picture.Link : "",
            //                 CreatedAt = c.Account.CreatedAt,
            //                 IsFollowing = c.Account.Followers.Any(f =>
            //                     f.AccountId == accountId && f.FollowingId == c.Account.Id
            //                 ),
            //             },
            //         })
            //         .FirstOrDefault(),
            // })
            .ProjectTo<GetPostsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = AccountId }
            )
            .ToListAsync();

        var hasNextPage = posts.Count > pageSize;

        var result = posts.Take(pageSize).OrderByDescending(p => p.Score).ToList();

        var nextCursor = hasNextPage ? result.Last().CreatedAt : (DateTime?)null;

        return (result, nextCursor);
    }

    public async Task<(List<GetPostsResponse>, DateTime?)> GetMyPostsListAsync(
        DateTime? cursor,
        // Guid accountId,
        int pageSize
    )
    {
        var baseQuery = _unitOfWork
            .Posts.ReadOnly()
            .WhereDeletedIsNull()
            .WhereAccountId(AccountId)
            .WhereIf(cursor.HasValue, q => q.WhereCursorLessThan(cursor!.Value));

        var postsQuery = baseQuery.OrderByDescending(p => p.CreatedAt).Take(pageSize + 1);

        var posts = await postsQuery
            // .Select(p => new GetPostsResponse
            // {
            //     Id = p.Id,
            //     Link = p.Link,
            //     Content = p.Content,
            //     CreatedAt = p.CreatedAt,
            //     UpdatedAt = p.UpdatedAt,

            //     IsOwner = p.AccountId == accountId,

            //     LikeCount = p.PostLikes.Count(),
            //     CommentCount = p.Comments.Count(),

            //     IsLiked = p.PostLikes.Any(l => l.AccountId == accountId),

            //     Author = new AccountNameResponse
            //     {
            //         Id = p.Account.Id,
            //         Username = p.Account.Username,
            //         DisplayName = p.Account.DisplayName,
            //         Avatar = p.Account.Picture != null ? p.Account.Picture.Link : "",
            //         CreatedAt = p.Account.CreatedAt,
            //         IsFollowing = false, // The owner cannot follow themselves
            //     },

            //     PostPictures = p.Pictures.Select(pic => pic.Link).ToList(),

            //     LatestComment = p
            //         .Comments.Where(c => c.ParentCommentId == null)
            //         .OrderByDescending(c => c.CreatedAt)
            //         .Select(c => new PostLatestComment
            //         {
            //             Content = c.Content,
            //             CreatedAt = c.CreatedAt,
            //             Commenter = new AccountNameResponse
            //             {
            //                 Id = c.Account.Id,
            //                 Username = c.Account.Username,
            //                 DisplayName = c.Account.DisplayName,
            //                 Avatar = c.Account.Picture != null ? c.Account.Picture.Link : "",
            //                 CreatedAt = c.Account.CreatedAt,
            //                 IsFollowing = c.Account.Followers.Any(f =>
            //                     f.AccountId == accountId && f.FollowingId == c.Account.Id
            //                 ),
            //             },
            //         })
            //         .FirstOrDefault(),
            // })
            .ProjectTo<GetPostsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = AccountId }
            )
            .ToListAsync();

        var hasNextPage = posts.Count > pageSize;

        var result = posts.Take(pageSize).ToList();

        var nextCursor = hasNextPage ? result.Last().CreatedAt : (DateTime?)null;

        return (result, nextCursor);
    }

    public async Task<(List<GetPostsResponse>, DateTime?)> GetPostsByUsernameAsync(
        string username,
        DateTime? cursor,
        // Guid accountId,
        int pageSize
    )
    {
        var existingUser = await _unitOfWork
            .Accounts.ReadOnly()
            .WhereDeletedIsNull()
            .WhereUsername(username)
            .AnyAsync();

        if (!existingUser)
            throw new NotFoundException("NoAccount");

        var baseQuery = _unitOfWork
            .Posts.ReadOnly()
            .WhereDeletedIsNull()
            .WhereAccountUsername(username)
            .WhereIf(cursor.HasValue, q => q.WhereCursorLessThan(cursor!.Value));

        var postsQuery = baseQuery.OrderByDescending(p => p.CreatedAt).Take(pageSize + 1);

        var posts = await postsQuery
            // .Select(p => new GetPostsResponse
            // {
            //     Id = p.Id,
            //     Link = p.Link,
            //     Content = p.Content,
            //     CreatedAt = p.CreatedAt,
            //     UpdatedAt = p.UpdatedAt,

            //     IsOwner = p.AccountId == accountId,

            //     LikeCount = p.PostLikes.Count(),
            //     CommentCount = p.Comments.Count(),

            //     IsLiked = p.PostLikes.Any(l => l.AccountId == accountId),

            //     Author = new AccountNameResponse
            //     {
            //         Id = p.Account.Id,
            //         Username = p.Account.Username,
            //         DisplayName = p.Account.DisplayName,
            //         Avatar = p.Account.Picture != null ? p.Account.Picture.Link : "",
            //         CreatedAt = p.Account.CreatedAt,
            //         IsFollowing = p.Account.Followers.Any(f =>
            //             f.AccountId == accountId && f.FollowingId == p.AccountId
            //         ),
            //     },

            //     PostPictures = p.Pictures.Select(pic => pic.Link).ToList(),

            //     LatestComment = p
            //         .Comments.Where(c => c.ParentCommentId == null)
            //         .OrderByDescending(c => c.CreatedAt)
            //         .Select(c => new PostLatestComment
            //         {
            //             Content = c.Content,
            //             CreatedAt = c.CreatedAt,
            //             Commenter = new AccountNameResponse
            //             {
            //                 Id = c.Account.Id,
            //                 Username = c.Account.Username,
            //                 DisplayName = c.Account.DisplayName,
            //                 Avatar = c.Account.Picture != null ? c.Account.Picture.Link : "",
            //                 CreatedAt = c.Account.CreatedAt,
            //                 IsFollowing = c.Account.Followers.Any(f =>
            //                     f.AccountId == accountId && f.FollowingId == c.Account.Id
            //                 ),
            //             },
            //         })
            //         .FirstOrDefault(),
            // })
            .ProjectTo<GetPostsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = AccountId }
            )
            .ToListAsync();

        var hasNextPage = posts.Count > pageSize;

        var result = posts.Take(pageSize).ToList();

        var nextCursor = hasNextPage ? result.Last().CreatedAt : (DateTime?)null;

        return (result, nextCursor);
    }

    public async Task<GetPostDetailResponse> GetPostByLinkAsync(string link)
    {
        var accountId = _jwtService.GetAccountInfo().Id;

        // return await _unitOfWork
        //     .Posts.ReadOnly()
        //     .WhereDeletedIsNull()
        //     .WhereLink(link)
        //     .Select(p => new GetPostDetailResponse
        //     {
        //         Id = p.Id,
        //         Link = p.Link,
        //         Content = p.Content,
        //         CreatedAt = p.CreatedAt,

        //         IsOwner = p.AccountId == accountId,
        //         IsLiked = p.PostLikes.Any(l => l.AccountId == accountId),

        //         LikeCount = p.PostLikes.Count(),
        //         CommentCount = p.Comments.Count(),

        //         PostPictures = p.Pictures.Select(pic => pic.Link).ToList(),

        //         Author = new AccountNameResponse
        //         {
        //             Id = p.Account.Id,
        //             Username = p.Account.Username,
        //             DisplayName = p.Account.DisplayName,
        //             Avatar = p.Account.Picture != null ? p.Account.Picture.Link : "",
        //             CreatedAt = p.Account.CreatedAt,
        //             IsFollowing = p.Account.Followers.Any(f =>
        //                 f.AccountId == accountId && f.FollowingId == p.AccountId
        //             ),
        //         },
        //     })
        //     .FirstOrDefaultAsync();

        return await _unitOfWork
                .Posts.ReadOnly()
                .WhereDeletedIsNull()
                .WhereLink(link)
                .ProjectTo<GetPostDetailResponse>(
                    _mapper.ConfigurationProvider,
                    new { currentAccId = AccountId }
                )
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException("NoPost");
    }

    private async Task<bool> IsPostExists(Guid postId)
    {
        return await _unitOfWork.Posts.ReadOnly().WhereDeletedIsNull().WhereId(postId).AnyAsync();
    }

    public async Task<int> LikePostAsync(Guid postId)
    {
        var postExists = await IsPostExists(postId);

        if (!postExists)
            throw new NotFoundException("NoPost");

        var alreadyLiked = await _unitOfWork
            .PostLikes.ReadOnly()
            .WhereAccountId(AccountId)
            .WherePostId(postId)
            .AnyAsync();

        if (!alreadyLiked)
        {
            _unitOfWork.PostLikes.Add(
                new PostLike
                {
                    PostId = postId,
                    AccountId = AccountId,
                    CreatedAt = DateTime.UtcNow,
                }
            );

            await _unitOfWork.SaveChangesAsync();
        }

        return await _unitOfWork.PostLikes.ReadOnly().WherePostId(postId).CountAsync();
    }

    public async Task<int> CancelLikePostAsync(Guid postId)
    {
        var postExists = await IsPostExists(postId);

        if (!postExists)
            throw new NotFoundException("NoPost");

        var alreadyLiked = await _unitOfWork
            .PostLikes.GetQuery()
            .WhereAccountId(AccountId)
            .WherePostId(postId)
            .FirstOrDefaultAsync();

        if (alreadyLiked != null)
        {
            _unitOfWork.PostLikes.Remove(alreadyLiked);

            await _unitOfWork.SaveChangesAsync();
        }

        return await _unitOfWork.PostLikes.ReadOnly().WherePostId(postId).CountAsync();
    }

    public async Task<(List<GetCommentsResponse>, DateTime?)> GetPostCommentsListAsync(
        Guid postId,
        DateTime? cursor,
        // Guid accountId,
        int pageSize
    )
    {
        var existingPost = await IsPostExists(postId);

        if (!existingPost)
            throw new NotFoundException("NoPost");

        var baseQuery = _unitOfWork
            .Comments.ReadOnly()
            .WhereDeletedIsNull()
            .WherePostId(postId)
            .WhereParentId(null)
            .WhereIf(cursor.HasValue, q => q.WhereCursorLessThan(cursor!.Value));

        var commmentsQuery = baseQuery.OrderByDescending(c => c.CreatedAt).Take(pageSize + 1);

        var comments = commmentsQuery
            // .Select(c => new GetCommentsResponse
            // {
            //     Id = c.Id,
            //     ParentCommentId = null,
            //     PostId = c.PostId,
            //     Content = c.Content,
            //     Commenter = new AccountNameResponse
            //     {
            //         Id = c.Account.Id,
            //         Username = c.Account.Username,
            //         DisplayName = c.Account.DisplayName,
            //         Avatar = c.Account.Picture != null ? c.Account.Picture.Link : "",
            //         CreatedAt = c.Account.CreatedAt,
            //         IsFollowing = c.Account.Followers.Any(f =>
            //             f.AccountId == accountId && f.FollowingId == c.AccountId
            //         ),
            //     },
            //     ReplyAccount = null,
            //     CreatedAt = c.CreatedAt,
            //     UpdatedAt = c.UpdatedAt,
            //     Pictures = c.Pictures.Select(cp => cp.Link).ToList(),
            //     LikeCount = c.CommentLikes.Count(),
            //     CommentCount = c.Replies.Count(),
            //     IsLiked = c.CommentLikes.Any(cl => cl.AccountId == accountId),
            // })
            .ProjectTo<GetCommentsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = AccountId }
            )
            .ToList();

        var hasNextPage = comments.Count > pageSize;

        var result = comments.Take(pageSize).ToList();

        var nextCursor = hasNextPage ? result.Last().CreatedAt : (DateTime?)null;

        return (result, nextCursor);
    }

    public async Task<GetPostsResponse> AddPostAsync(CreatePostRequest request)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Already checked in the middleware

            // var existingAccount = await _unitOfWork
            //     .Accounts.ReadOnly()
            //     .WhereDeletedIsNull()
            //     .WhereId(accountId)
            //     .AnyAsync();

            // if (!existingAccount)
            //     throw new UnauthorizedException("NoAccount");

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Link = StringHelper.GenerateRandomString(_settings.TokenLength),
                Content = request.Content,
                AccountId = AccountId,
                CreatedAt = DateTime.UtcNow,
            };

            _unitOfWork.Posts.Add(post);

            await _unitOfWork.SaveChangesAsync();

            if (request.Pictures.Count != 0)
            {
                await _unitOfWork
                    .Pictures.GetQuery()
                    .Where(p => request.Pictures.Contains(p.Link))
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.PostId, post.Id));
            }

            await _unitOfWork.CommitTransactionAsync();

            return await _unitOfWork
                .Posts.ReadOnly()
                .WhereId(post.Id)
                .ProjectTo<GetPostsResponse>(
                    _mapper.ConfigurationProvider,
                    new { currentAccId = AccountId }
                )
                .FirstAsync();

            // return new GetPostsResponse
            // {
            //     Id = post.Id,
            //     Link = post.Link,
            //     Content = post.Content,
            //     CreatedAt = post.CreatedAt,
            //     UpdatedAt = post.UpdatedAt,

            //     IsOwner = true,
            //     IsLiked = false,

            //     LikeCount = 0,
            //     CommentCount = 0,

            //     PostPictures = request.Pictures ?? [],

            //     Author = new AccountNameResponse
            //     {
            //         Id = accountId,
            //         Username = existingAccount.Username,
            //         DisplayName = existingAccount.DisplayName,
            //         Avatar = existingAccount.Avatar,
            //         CreatedAt = post.CreatedAt,
            //         IsFollowing = false,
            //     },
            // };
            // return response;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GetPostsResponse> UpdatePostAsync(UpdatePostRequest request, Guid postId)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var existingPost =
                await _unitOfWork
                    .Posts.GetQuery()
                    .WhereDeletedIsNull()
                    .WhereId(postId)
                    .WhereAccountId(AccountId)
                    .FirstOrDefaultAsync()
                ?? throw new NotFoundException("NoPost");

            var updateTime = DateTime.UtcNow;

            await _unitOfWork
                .Posts.GetQuery()
                .Where(p => p.Id == postId)
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(p => p.Content, request.Content)
                        .SetProperty(p => p.UpdatedAt, updateTime)
                );

            await _unitOfWork
                .Pictures.GetQuery()
                .WherePostId(postId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.PostId, (Guid?)null));

            if (request.Pictures?.Count > 0)
            {
                await _unitOfWork
                    .Pictures.GetQuery()
                    .Where(p => request.Pictures.Contains(p.Link))
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.PostId, postId));
            }

            await _unitOfWork.CommitTransactionAsync();

            return await _unitOfWork
                .Posts.ReadOnly()
                .WhereId(postId)
                .ProjectTo<GetPostsResponse>(
                    _mapper.ConfigurationProvider,
                    new { currentAccId = AccountId }
                )
                .FirstAsync();

            //   IsLiked = existingPost.PostLikes.Any(l => l.AccountId == accountId),

            // return new GetPostsResponse
            // {
            //     Id = existingPost.Id,
            //     Link = existingPost.Link,
            //     Content = request.Content,
            //     CreatedAt = existingPost.CreatedAt,
            //     UpdatedAt = updateTime,

            //     IsOwner = true,
            //     IsLiked = existingPost.PostLikes.Any(l => l.AccountId == accountId),

            //     LikeCount = existingPost.PostLikes.Count,
            //     CommentCount = existingPost.Comments.Count,

            //     PostPictures = request.Pictures ?? [],

            //     Author = new AccountNameResponse
            //     {
            //         Id = accountId,
            //         Username = author.Username,
            //         DisplayName = author.DisplayName,
            //         Avatar = author.Avatar,
            //         CreatedAt = author.CreatedAt,
            //         IsFollowing = false,
            //     },
            // };

            // return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task DeletePostAsync(Guid postId)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var deletedAt = DateTime.UtcNow;

            var affectedRows = await _unitOfWork
                .Posts.GetQuery()
                .WhereDeletedIsNull()
                .WhereId(postId)
                .WhereAccountId(AccountId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.DeletedAt, deletedAt));

            if (affectedRows == 0)
                throw new NotFoundException("NoPost");

            await _unitOfWork
                .Pictures.GetQuery()
                .WherePostId(postId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.PostId, (Guid?)null));

            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
