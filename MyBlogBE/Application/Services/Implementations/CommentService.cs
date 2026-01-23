using Application.Dtos;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessObject.Entities;
using DataAccess.Extensions;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<(List<GetCommentsResponse>?, DateTime?)> GetChildCommentList(
        Guid commentId,
        DateTime? cursor,
        Guid accountId,
        int pageSize
    )
    {
        var baseQuery = _unitOfWork
            .Comments.ReadOnly()
            .WhereDeletedIsNull()
            // .WhereReplyAccountId(null)
            .WhereParentId(commentId)
            .WhereIf(cursor != null, q => q.WhereCursorGreaterThan(cursor!.Value));

        if (!await baseQuery.AnyAsync())
        {
            return (null, null);
        }

        var commentQuery = baseQuery.OrderBy(c => c.CreatedAt).Take(pageSize + 1);

        var comments = await commentQuery
            // .Select(c => new GetCommentsResponse
            // {
            //     Id = c.Id,
            //     Content = c.Content,
            //     Commenter = new AccountNameResponse
            //     {
            //         Id = c.Account.Id,
            //         Username = c.Account.Username,
            //         DisplayName = c.Account.DisplayName,
            //         Avatar = c.Account.Picture != null ? c.Account.Picture.Link : string.Empty,
            //     },
            //     ReplyAccount =
            //         c.ReplyAccount != null
            //             ? new AccountNameResponse
            //             {
            //                 Id = c.ReplyAccount.Id,
            //                 Username = c.ReplyAccount.Username,
            //                 DisplayName = c.ReplyAccount.DisplayName,
            //                 Avatar =
            //                     c.ReplyAccount.Picture != null
            //                         ? c.ReplyAccount.Picture.Link
            //                         : string.Empty,
            //             }
            //             : null,
            //     CreatedAt = c.CreatedAt,
            //     Pictures = c.Pictures.Select(cp => cp.Link).ToList(),
            //     LikeCount = c.CommentLikes.Count(),
            //     CommentCount = c.Replies.Count(),
            //     IsLiked = c.CommentLikes.Any(cl => cl.AccountId == accountId),
            //     PostId = c.PostId,
            //     ParentCommentId = c.ParentCommentId,
            //     UpdatedAt = c.UpdatedAt,
            // })
            .ProjectTo<GetCommentsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = accountId }
            )
            .ToListAsync();

        var hasMore = comments.Count > pageSize;

        var res = comments.Take(pageSize).ToList();

        DateTime? nextCursor = hasMore ? res.Last().CreatedAt : null;

        return (res, nextCursor);
    }

    private async Task<bool> IsCommentsExists(Guid commentId)
    {
        return await _unitOfWork
            .Comments.ReadOnly()
            .WhereDeletedIsNull()
            .WhereId(commentId)
            .AnyAsync();
    }

    public async Task<int?> LikeCommentAsync(Guid commentId, Guid accountId)
    {
        var existingComment = await IsCommentsExists(commentId);

        if (!existingComment)
        {
            return null;
        }

        var existingLike = await _unitOfWork
            .CommentLikes.GetQuery()
            .WhereCommentId(commentId)
            .WhereAccountId(accountId)
            .FirstOrDefaultAsync();

        if (existingLike == null)
        {
            _unitOfWork.CommentLikes.Add(
                new CommentLike
                {
                    CommentId = commentId,
                    AccountId = accountId,
                    CreatedAt = DateTime.UtcNow,
                }
            );

            await _unitOfWork.SaveChangesAsync();
        }

        return await _unitOfWork.CommentLikes.ReadOnly().WhereCommentId(commentId).CountAsync();
    }

    public async Task<int?> CancelLikeCommentAsync(Guid commentId, Guid accountId)
    {
        var existingComment = await IsCommentsExists(commentId);

        if (!existingComment)
        {
            return null;
        }

        var existingLike = await _unitOfWork
            .CommentLikes.GetQuery()
            .WhereCommentId(commentId)
            .WhereAccountId(accountId)
            .FirstOrDefaultAsync();

        if (existingLike != null)
        {
            _unitOfWork.CommentLikes.Remove(existingLike);
            await _unitOfWork.SaveChangesAsync();
        }

        return await _unitOfWork.CommentLikes.ReadOnly().WhereCommentId(commentId).CountAsync();
    }

    public async Task<GetCommentsResponse?> AddCommentAsync(
        Guid accountId,
        CreateCommentRequest request
    )
    {
        var post = await _unitOfWork
            .Posts.ReadOnly()
            .WhereDeletedIsNull()
            .WhereId(request.PostId)
            .FirstOrDefaultAsync();

        if (post == null)
            return null;

        if (request.ParentCommentId != null)
        {
            var parentComment = await _unitOfWork
                .Comments.ReadOnly()
                .WhereDeletedIsNull()
                .WherePostId(request.PostId)
                .WhereId(request.ParentCommentId.Value)
                .FirstOrDefaultAsync();

            if (parentComment == null)
                return null;
        }

        AccountNameResponse? replyAccount = null;

        if (request.ReplyAccountId != null)
        {
            replyAccount = await _unitOfWork
                .Accounts.ReadOnly()
                .WhereId(request.ReplyAccountId.Value)
                .Select(a => new AccountNameResponse
                {
                    Id = a.Id,
                    Username = a.Username,
                    DisplayName = a.DisplayName,
                    Avatar = a.Picture != null ? a.Picture.Link : string.Empty,
                })
                .FirstOrDefaultAsync();

            if (replyAccount == null)
                return null;
        }

        var commenter = await _unitOfWork
            .Accounts.ReadOnly()
            .WhereId(accountId)
            .Select(a => new AccountNameResponse
            {
                Id = a.Id,
                Username = a.Username,
                DisplayName = a.DisplayName,
                Avatar = a.Picture != null ? a.Picture.Link : string.Empty,
            })
            .FirstAsync();

        var now = DateTime.UtcNow;
        var commentId = Guid.NewGuid();

        await _unitOfWork.BeginTransactionAsync();

        var comment = new Comment
        {
            Id = commentId,
            AccountId = commenter.Id,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            PostId = request.PostId,
            ReplyAccountId = request.ReplyAccountId,
            CreatedAt = now,
        };

        _unitOfWork.Comments.Add(comment);
        await _unitOfWork.SaveChangesAsync();

        if (request.Pictures.Count > 0)
        {
            await _unitOfWork
                .Pictures.GetQuery()
                .Where(p => request.Pictures.Contains(p.Link))
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.CommentId, comment.Id));
        }

        await _unitOfWork.CommitTransactionAsync();

        var result = await _unitOfWork
            .Comments.ReadOnly()
            .WhereId(commentId)
            .WhereDeletedIsNull()
            .ProjectTo<GetCommentsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = accountId }
            )
            .FirstAsync();

        return result;
        // return new GetCommentsResponse
        // {
        //     Id = comment.Id,
        //     Commenter = commenter,
        //     Content = comment.Content,
        //     ParentCommentId = comment.ParentCommentId,
        //     PostId = comment.PostId,
        //     ReplyAccount = replyAccount,
        //     LikeCount = 0,
        //     CommentCount = 0,
        //     IsLiked = false,
        //     Pictures = request.Pictures ?? [],
        //     CreatedAt = comment.CreatedAt,
        //     UpdatedAt = null,
        // };
    }

    public async Task<GetCommentsResponse?> UpdateCommentAsync(
        Guid commentId,
        UpdateCommentRequest request,
        Guid accountId
    )
    {
        var existingComment = await _unitOfWork
            .Comments.GetQuery()
            .WhereDeletedIsNull()
            .WhereId(commentId)
            .WhereAccountId(accountId)
            .FirstOrDefaultAsync();

        if (existingComment == null)
        {
            return null;
        }

        await _unitOfWork.BeginTransactionAsync();

        // Update comment content
        existingComment.Content = request.Content;
        existingComment.UpdatedAt = DateTime.UtcNow;

        // Clear existing pictures
        await _unitOfWork
            .Pictures.GetQuery()
            .WhereCommentId(commentId)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.CommentId, (Guid?)null));

        if (request.Pictures.Count > 0)
        {
            await _unitOfWork
                .Pictures.GetQuery()
                .Where(p => request.Pictures.Contains(p.Link))
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.CommentId, commentId));
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        var result = await _unitOfWork
            .Comments.ReadOnly()
            .WhereId(commentId)
            .WhereDeletedIsNull()
            .ProjectTo<GetCommentsResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = accountId }
            )
            .FirstAsync();

        return result;

        // var res = await _unitOfWork
        //     .Comments.ReadOnly()
        //     .WhereDeletedIsNull()
        //     .WhereId(commentId)
        //     .WhereAccountId(accountId)
        //     .Select(c => new GetCommentsResponse
        //     {
        //         Id = c.Id,
        //         Content = c.Content,
        //         CreatedAt = c.CreatedAt,
        //         UpdatedAt = c.UpdatedAt,
        //         ParentCommentId = c.ParentCommentId,
        //         PostId = c.PostId,

        //         CommentCount = c.Replies.Count(),
        //         LikeCount = c.CommentLikes.Count(),
        //         IsLiked = c.CommentLikes.Any(l => l.AccountId == accountId),

        //         Commenter = new AccountNameResponse
        //         {
        //             Id = c.Account.Id,
        //             Username = c.Account.Username,
        //             DisplayName = c.Account.DisplayName,
        //             Avatar = c.Account.Picture != null ? c.Account.Picture.Link : string.Empty,
        //         },

        //         ReplyAccount =
        //             c.ReplyAccount != null
        //                 ? new AccountNameResponse
        //                 {
        //                     Id = c.ReplyAccount.Id,
        //                     Username = c.ReplyAccount.Username,
        //                     DisplayName = c.ReplyAccount.DisplayName,
        //                     Avatar =
        //                         c.ReplyAccount.Picture != null
        //                             ? c.ReplyAccount.Picture.Link
        //                             : string.Empty,
        //                 }
        //                 : null,

        //         Pictures = c.Pictures.Select(p => p.Link).ToList(),
        //     })
        //     .FirstAsync();

        // return res;
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId, Guid accountId)
    {
        var existingComment = await _unitOfWork
            .Comments.GetQuery()
            .WhereDeletedIsNull()
            .WhereId(commentId)
            .WhereAccountId(accountId)
            .FirstOrDefaultAsync();

        if (existingComment == null)
        {
            return false;
        }

        await _unitOfWork.BeginTransactionAsync();

        existingComment.DeletedAt = DateTime.UtcNow;

        await _unitOfWork
            .Pictures.GetQuery()
            .WhereCommentId(commentId)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.CommentId, (Guid?)null));

        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.CommitTransactionAsync();

        return true;
    }
}
