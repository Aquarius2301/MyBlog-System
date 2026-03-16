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
        CreateTarotCard(context);
    }

    private static void CreateTarotCard(MyBlogContext context)
    {
        if (!context.TarotCards.Any())
        {
            Console.WriteLine("Seeding tarot cards...");

            var TarotCard = new List<TarotCard>
            {
                // ===== Major Arcana =====
                new TarotCard { Name = "The Fool", ImageUrl = "/tarot/the_fool.jpg" },
                new TarotCard { Name = "The Magician", ImageUrl = "/tarot/the_magician.jpg" },
                new TarotCard
                {
                    Name = "The High Priestess",
                    ImageUrl = "/tarot/the_high_priestess.jpg",
                },
                new TarotCard { Name = "The Empress", ImageUrl = "/tarot/the_empress.jpg" },
                new TarotCard { Name = "The Emperor", ImageUrl = "/tarot/the_emperor.jpg" },
                new TarotCard { Name = "The Hierophant", ImageUrl = "/tarot/the_hierophant.jpg" },
                new TarotCard { Name = "The Lovers", ImageUrl = "/tarot/the_lovers.jpg" },
                new TarotCard { Name = "The Chariot", ImageUrl = "/tarot/the_chariot.jpg" },
                new TarotCard { Name = "Strength", ImageUrl = "/tarot/strength.jpg" },
                new TarotCard { Name = "The Hermit", ImageUrl = "/tarot/the_hermit.jpg" },
                new TarotCard
                {
                    Name = "Wheel of Fortune",
                    ImageUrl = "/tarot/wheel_of_fortune.jpg",
                },
                new TarotCard { Name = "Justice", ImageUrl = "/tarot/justice.jpg" },
                new TarotCard { Name = "The Hanged Man", ImageUrl = "/tarot/the_hanged_man.jpg" },
                new TarotCard { Name = "Death", ImageUrl = "/tarot/death.jpg" },
                new TarotCard { Name = "Temperance", ImageUrl = "/tarot/temperance.jpg" },
                new TarotCard { Name = "The Devil", ImageUrl = "/tarot/the_devil.jpg" },
                new TarotCard { Name = "The Tower", ImageUrl = "/tarot/the_tower.jpg" },
                new TarotCard { Name = "The Star", ImageUrl = "/tarot/the_star.jpg" },
                new TarotCard { Name = "The Moon", ImageUrl = "/tarot/the_moon.jpg" },
                new TarotCard { Name = "The Sun", ImageUrl = "/tarot/the_sun.jpg" },
                new TarotCard { Name = "Judgement", ImageUrl = "/tarot/judgement.jpg" },
                new TarotCard { Name = "The World", ImageUrl = "/tarot/the_world.jpg" },
                // ===== Minor Arcana – Wands =====
                new TarotCard { Name = "Ace of Wands", ImageUrl = "/tarot/ace_of_wands.jpg" },
                new TarotCard { Name = "Two of Wands", ImageUrl = "/tarot/two_of_wands.jpg" },
                new TarotCard { Name = "Three of Wands", ImageUrl = "/tarot/three_of_wands.jpg" },
                new TarotCard { Name = "Four of Wands", ImageUrl = "/tarot/four_of_wands.jpg" },
                new TarotCard { Name = "Five of Wands", ImageUrl = "/tarot/five_of_wands.jpg" },
                new TarotCard { Name = "Six of Wands", ImageUrl = "/tarot/six_of_wands.jpg" },
                new TarotCard { Name = "Seven of Wands", ImageUrl = "/tarot/seven_of_wands.jpg" },
                new TarotCard { Name = "Eight of Wands", ImageUrl = "/tarot/eight_of_wands.jpg" },
                new TarotCard { Name = "Nine of Wands", ImageUrl = "/tarot/nine_of_wands.jpg" },
                new TarotCard { Name = "Ten of Wands", ImageUrl = "/tarot/ten_of_wands.jpg" },
                new TarotCard { Name = "Page of Wands", ImageUrl = "/tarot/page_of_wands.jpg" },
                new TarotCard { Name = "Knight of Wands", ImageUrl = "/tarot/knight_of_wands.jpg" },
                new TarotCard { Name = "Queen of Wands", ImageUrl = "/tarot/queen_of_wands.jpg" },
                new TarotCard { Name = "King of Wands", ImageUrl = "/tarot/king_of_wands.jpg" },
                // ===== Minor Arcana – Cups =====
                new TarotCard { Name = "Ace of Cups", ImageUrl = "/tarot/ace_of_cups.jpg" },
                new TarotCard { Name = "Two of Cups", ImageUrl = "/tarot/two_of_cups.jpg" },
                new TarotCard { Name = "Three of Cups", ImageUrl = "/tarot/three_of_cups.jpg" },
                new TarotCard { Name = "Four of Cups", ImageUrl = "/tarot/four_of_cups.jpg" },
                new TarotCard { Name = "Five of Cups", ImageUrl = "/tarot/five_of_cups.jpg" },
                new TarotCard { Name = "Six of Cups", ImageUrl = "/tarot/six_of_cups.jpg" },
                new TarotCard { Name = "Seven of Cups", ImageUrl = "/tarot/seven_of_cups.jpg" },
                new TarotCard { Name = "Eight of Cups", ImageUrl = "/tarot/eight_of_cups.jpg" },
                new TarotCard { Name = "Nine of Cups", ImageUrl = "/tarot/nine_of_cups.jpg" },
                new TarotCard { Name = "Ten of Cups", ImageUrl = "/tarot/ten_of_cups.jpg" },
                new TarotCard { Name = "Page of Cups", ImageUrl = "/tarot/page_of_cups.jpg" },
                new TarotCard { Name = "Knight of Cups", ImageUrl = "/tarot/knight_of_cups.jpg" },
                new TarotCard { Name = "Queen of Cups", ImageUrl = "/tarot/queen_of_cups.jpg" },
                new TarotCard { Name = "King of Cups", ImageUrl = "/tarot/king_of_cups.jpg" },
                // ===== Minor Arcana – Swords =====
                new TarotCard { Name = "Ace of Swords", ImageUrl = "/tarot/ace_of_swords.jpg" },
                new TarotCard { Name = "Two of Swords", ImageUrl = "/tarot/two_of_swords.jpg" },
                new TarotCard { Name = "Three of Swords", ImageUrl = "/tarot/three_of_swords.jpg" },
                new TarotCard { Name = "Four of Swords", ImageUrl = "/tarot/four_of_swords.jpg" },
                new TarotCard { Name = "Five of Swords", ImageUrl = "/tarot/five_of_swords.jpg" },
                new TarotCard { Name = "Six of Swords", ImageUrl = "/tarot/six_of_swords.jpg" },
                new TarotCard { Name = "Seven of Swords", ImageUrl = "/tarot/seven_of_swords.jpg" },
                new TarotCard { Name = "Eight of Swords", ImageUrl = "/tarot/eight_of_swords.jpg" },
                new TarotCard { Name = "Nine of Swords", ImageUrl = "/tarot/nine_of_swords.jpg" },
                new TarotCard { Name = "Ten of Swords", ImageUrl = "/tarot/ten_of_swords.jpg" },
                new TarotCard { Name = "Page of Swords", ImageUrl = "/tarot/page_of_swords.jpg" },
                new TarotCard
                {
                    Name = "Knight of Swords",
                    ImageUrl = "/tarot/knight_of_swords.jpg",
                },
                new TarotCard { Name = "Queen of Swords", ImageUrl = "/tarot/queen_of_swords.jpg" },
                new TarotCard { Name = "King of Swords", ImageUrl = "/tarot/king_of_swords.jpg" },
                // ===== Minor Arcana – Pentacles =====
                new TarotCard
                {
                    Name = "Ace of Pentacles",
                    ImageUrl = "/tarot/ace_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Two of Pentacles",
                    ImageUrl = "/tarot/two_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Three of Pentacles",
                    ImageUrl = "/tarot/three_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Four of Pentacles",
                    ImageUrl = "/tarot/four_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Five of Pentacles",
                    ImageUrl = "/tarot/five_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Six of Pentacles",
                    ImageUrl = "/tarot/six_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Seven of Pentacles",
                    ImageUrl = "/tarot/seven_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Eight of Pentacles",
                    ImageUrl = "/tarot/eight_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Nine of Pentacles",
                    ImageUrl = "/tarot/nine_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Ten of Pentacles",
                    ImageUrl = "/tarot/ten_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Page of Pentacles",
                    ImageUrl = "/tarot/page_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Knight of Pentacles",
                    ImageUrl = "/tarot/knight_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "Queen of Pentacles",
                    ImageUrl = "/tarot/queen_of_pentacles.jpg",
                },
                new TarotCard
                {
                    Name = "King of Pentacles",
                    ImageUrl = "/tarot/king_of_pentacles.jpg",
                },
            };

            context.TarotCards.AddRange(TarotCard);
            context.SaveChanges();
        }
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
