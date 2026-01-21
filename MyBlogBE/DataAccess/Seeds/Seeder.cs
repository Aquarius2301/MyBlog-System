using BusinessObject.Entities;
using BusinessObject.Enums;

namespace DataAccess.Seeds;

public class Seeder
{
    public static void Seed(MyBlogContext context)
    {
        CreateAccounts(context);
        CreateFollows(context);
        CreatePosts(context);
        CreatePostLikes(context);
        CreateComments(context);
    }

    private static void CreateAccounts(MyBlogContext context)
    {
        if (!context.Accounts.Any())
        {
            Console.WriteLine("Seeding accounts...");
            var accounts = new List<Account>();
            for (int i = 0; i < 20; i++)
            {
                var id = Guid.NewGuid();
                accounts.Add(
                    new Account
                    {
                        Id = id,
                        Username = $"user{i + 1}",
                        DisplayName = $"User {i + 1}",
                        DateOfBirth = new DateOnly(1990 + (i % 10), 1 + (i % 12), 1 + (i % 28)),
                        HashedPassword =
                            "100000.2WBYMJzOMwL6A4WFMqocgA==.mOhKh2DlCdEv5kF51VSfWo9ddeeeayxz9kH7lwI4EAI=",
                        Status = StatusType.Active.Code,
                        Email = $"user{i + 1}@example.com",
                        Picture = new Picture
                        {
                            Id = Guid.NewGuid(),
                            AccountId = id,
                            PublicId = $"profile_user{i + 1}",
                            Link = $"/profiles/user{i + 1}/avatar.jpg",
                        },
                        CreatedAt = DateTime.UtcNow.AddDays(Random.Shared.Next(-100, 0)),
                    }
                );
            }

            context.Accounts.AddRange(accounts);
            context.SaveChanges();
        }
    }

    private static void CreatePosts(MyBlogContext context)
    {
        if (!context.Posts.Any())
        {
            Console.WriteLine("Seeding posts...");

            var posts = new List<Post>();
            var accounts = context.Accounts.ToList();

            foreach (var account in accounts)
            {
                var numOfPosts = Random.Shared.Next(1, 6);
                for (int i = 0; i < numOfPosts; i++)
                {
                    var id = Guid.NewGuid();
                    var PostPicsCount = Random.Shared.Next(1, 3);
                    posts.Add(
                        new Post
                        {
                            Id = id,
                            Link = $"post{account.Username}{i + 1}",
                            Content = $"This is post {i + 1} by {account.DisplayName}.",
                            AccountId = account.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(Random.Shared.Next(-100, 0)),
                            Pictures = Enumerable
                                .Range(1, PostPicsCount)
                                .Select(j => new Picture
                                {
                                    Id = Guid.NewGuid(),
                                    PostId = id,
                                    PublicId = $"post_{account.Username}_{id}_pic{j}",
                                    Link = $"/posts/{account.Username}/post_{id}/picture{j}.jpg",
                                })
                                .ToList(),
                        }
                    );
                }
            }

            context.Posts.AddRange(posts);
            context.SaveChanges();
        }
    }

    private static void CreatePostLikes(MyBlogContext context)
    {
        if (!context.PostLikes.Any())
        {
            Console.WriteLine("Seeding post likes...");

            var postLikes = new List<PostLike>();
            var posts = context.Posts.ToList();
            var accounts = context.Accounts.ToList();
            var rand = new Random();

            foreach (var post in posts)
            {
                int likeCount = rand.Next(1, 4);

                var likedAccounts = accounts.OrderBy(a => rand.Next()).Take(likeCount).ToList();

                foreach (var acc in likedAccounts)
                {
                    postLikes.Add(
                        new PostLike
                        {
                            Id = Guid.NewGuid(),
                            PostId = post.Id,
                            AccountId = acc.Id,
                            CreatedAt = DateTime.UtcNow.AddDays(Random.Shared.Next(-100, 0)),
                        }
                    );
                }
            }

            context.PostLikes.AddRange(postLikes);
            context.SaveChanges();
        }
    }

    private static void CreateFollows(MyBlogContext context)
    {
        if (!context.Follows.Any())
        {
            Console.WriteLine("Seeding follows...");

            var existingAccounts = context.Accounts.ToList();
            var follows = new List<Follow>();
            var rand = new Random();

            foreach (var acc in existingAccounts)
            {
                int followCount = rand.Next(1, 4);

                var followAccounts = existingAccounts
                    .OrderBy(a => rand.Next())
                    .Take(followCount)
                    .ToList();

                foreach (var followAcc in followAccounts)
                {
                    follows.Add(
                        new Follow
                        {
                            Id = Guid.NewGuid(),
                            FollowingId = followAcc.Id,
                            AccountId = acc.Id,
                        }
                    );
                }
            }

            context.Follows.AddRange(follows);
            context.SaveChanges();
        }
    }

    private static void CreateComments(MyBlogContext context)
    {
        if (!context.Comments.Any())
        {
            Console.WriteLine("Seeding comments...");

            var rand = new Random();

            var posts = context.Posts.ToList();
            var accounts = context.Accounts.ToList();

            foreach (var post in posts)
            {
                var comments = new List<Comment>();
                var commentCount = rand.Next(1, 5);
                for (int i = 0; i < commentCount; i++)
                {
                    var account = accounts[rand.Next(accounts.Count)];
                    var commentPicsCount = rand.Next(1, 3);
                    var id = Guid.NewGuid();

                    var parentComment = new Comment
                    {
                        Id = id,
                        AccountId = account.Id,
                        ParentCommentId = null,
                        ReplyAccountId = null,
                        PostId = post.Id,
                        Pictures = Enumerable
                            .Range(1, commentPicsCount)
                            .Select(j => new Picture
                            {
                                Id = Guid.NewGuid(),
                                PublicId = $"comment_{account.Username}_{id}_pic{j}",
                                CommentId = id,
                                Link = $"/comments/{account.Username}/comment_{id}/picture{j}.jpg",
                            })
                            .ToList(),
                        Content = $"Parent comment from {account.Username}",
                        CreatedAt = DateTime.UtcNow.AddDays(-rand.Next(100)),
                    };

                    context.Comments.Add(parentComment);
                    context.SaveChanges();

                    int childCount = rand.Next(1, 10);
                    for (int j = 0; i < childCount; i++)
                    {
                        var childAccount = accounts[rand.Next(accounts.Count)];
                        var commentPicsChildCount = rand.Next(1, 3);
                        id = Guid.NewGuid();
                        var childComment = new Comment
                        {
                            Id = id,
                            AccountId = childAccount.Id,
                            ParentCommentId = parentComment.Id,
                            ReplyAccountId = parentComment.AccountId,
                            PostId = post.Id,
                            Pictures = Enumerable
                                .Range(1, commentPicsChildCount)
                                .Select(k => new Picture
                                {
                                    Id = Guid.NewGuid(),
                                    CommentId = id,
                                    PublicId = $"comment_{childAccount.Username}_{id}_pic{k}",
                                    Link =
                                        $"/comments/{childAccount.Username}/comment_{id}/picture{k}.jpg",
                                })
                                .ToList(),
                            Content = $"Reply comment {j + 1} from {childAccount.Username}",
                            CreatedAt = DateTime.UtcNow.AddDays(-rand.Next(100)),
                            UpdatedAt = DateTime.UtcNow.AddDays(-rand.Next(100)),
                        };

                        context.Comments.Add(childComment);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
