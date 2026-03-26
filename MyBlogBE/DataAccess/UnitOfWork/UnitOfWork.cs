using BusinessObject.Entities;
using DataAccess.Repositories;

namespace DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public IRepository<Account> Accounts { get; }
    public IRepository<Picture> Pictures { get; }
    public IRepository<Post> Posts { get; }
    public IRepository<PostLike> PostLikes { get; }
    public IRepository<Comment> Comments { get; }
    public IRepository<CommentLike> CommentLikes { get; }
    public IRepository<TarotCard> TarotCards { get; }
    public IRepository<Follow> Follows { get; }

    public IRepository<Conversation> Conversations { get; }

    // public IRepository<ConversationParticipant> ConversationParticipants { get; }
    public IRepository<Message> Messages { get; }
    private readonly MyBlogContext _context;

    public UnitOfWork(
        MyBlogContext context,
        IRepository<Account> accountRepository,
        IRepository<Picture> pictureRepository,
        IRepository<Post> postRepository,
        IRepository<PostLike> postLikeRepository,
        IRepository<Comment> commentRepository,
        IRepository<CommentLike> commentLikeRepository,
        IRepository<TarotCard> tarotCardRepository,
        IRepository<Follow> followRepository,
        IRepository<Conversation> conversationRepository,
        // IRepository<ConversationParticipant> conversationParticipantRepository,
        IRepository<Message> messageRepository
    )
    {
        _context = context;
        Accounts = accountRepository;
        Pictures = pictureRepository;
        Posts = postRepository;
        PostLikes = postLikeRepository;
        Comments = commentRepository;
        CommentLikes = commentLikeRepository;
        TarotCards = tarotCardRepository;
        Follows = followRepository;
        Conversations = conversationRepository;
        // ConversationParticipants = conversationParticipantRepository;
        Messages = messageRepository;
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
