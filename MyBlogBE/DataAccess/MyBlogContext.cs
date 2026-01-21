using BusinessObject.Entities;
using BusinessObject.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class MyBlogContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<Picture> Pictures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Account>()
            .HasMany(a => a.Posts)
            .WithOne(p => p.Account)
            .HasForeignKey(p => p.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Account>()
            .HasMany(a => a.Comments)
            .WithOne(p => p.Account)
            .HasForeignKey(p => p.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Account>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Account>().HasIndex(u => u.Email).IsUnique();

        modelBuilder
            .Entity<Account>()
            .HasOne(a => a.Picture)
            .WithOne(a => a.Account)
            .HasForeignKey<Account>(p => p.PictureId)
            .OnDelete(DeleteBehavior.NoAction);

        // modelBuilder.Entity<Account>(entity =>
        // {
        //     entity
        //         .Property(e => e.Status)
        //         .HasConversion(
        //             v => v.Code, // Lưu xuống DB là string (Code)
        //             v => new StatusType(v) // Đọc từ DB lên thì tạo mới record
        //         )
        //         .HasMaxLength(20);
        // });

        modelBuilder
            .Entity<Comment>()
            .HasOne(c => c.Account)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Comment>()
            .HasOne(c => c.ReplyAccount)
            .WithMany()
            .HasForeignKey(c => c.ReplyAccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(c => c.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Comment>()
            .HasMany(c => c.Pictures)
            .WithOne(c => c.Comment)
            .HasForeignKey(c => c.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<CommentLike>()
            .HasOne(c => c.Comment)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(c => c.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<CommentLike>()
            .HasOne(c => c.Account)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<CommentLike>()
            .HasIndex(u => new { u.CommentId, u.AccountId })
            .IsUnique();

        modelBuilder
            .Entity<Follow>()
            .HasOne(c => c.Account)
            .WithMany(c => c.Following)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Follow>()
            .HasOne(c => c.Following)
            .WithMany(c => c.Followers)
            .HasForeignKey(c => c.FollowingId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Follow>().HasIndex(u => new { u.AccountId, u.FollowingId }).IsUnique();

        modelBuilder
            .Entity<Picture>()
            .HasOne(c => c.Account)
            .WithOne(c => c.Picture)
            .HasForeignKey<Picture>(c => c.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Picture>()
            .HasOne(c => c.Post)
            .WithMany(c => c.Pictures)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Picture>()
            .HasOne(c => c.Comment)
            .WithMany(c => c.Pictures)
            .HasForeignKey(c => c.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Post>()
            .HasOne(c => c.Account)
            .WithMany(c => c.Posts)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Post>()
            .HasMany(c => c.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<Post>()
            .HasMany(c => c.Pictures)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Post>().HasIndex(u => u.Link).IsUnique();

        modelBuilder
            .Entity<PostLike>()
            .HasOne(c => c.Post)
            .WithMany(c => c.PostLikes)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
            .Entity<PostLike>()
            .HasOne(c => c.Account)
            .WithMany(c => c.PostLikes)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PostLike>().HasIndex(u => new { u.PostId, u.AccountId }).IsUnique();
    }

    public MyBlogContext(DbContextOptions<MyBlogContext> options)
        : base(options) { }
}
