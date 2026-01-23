using Application.Services.Interfaces;

namespace Application.Dtos;

public class GetPostsResponse : GetPostDetailResponse
{
    public PostLatestComment? LatestComment { get; set; } = null;
    public int Score =>
        (Author.IsFollowing ? 30 : 0) + LikeCount * 2 + CommentCount * 3 + PostPictures.Count;
}

public class PostLatestComment
{
    public AccountNameResponse Commenter { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class GetPostDetailResponse
{
    public Guid Id { get; set; }
    public string Link { get; set; } = null!;
    public string Content { get; set; } = null!;
    public AccountNameResponse Author { get; set; } = null!;
    public List<string> PostPictures { get; set; } = null!;
    public bool IsLiked { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsOwner { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = null;
}

public class CreatePostRequest
{
    public string Content { get; set; } = null!;
    public List<string> Pictures { get; set; } = [];
}

public class CreatePostResponse
{
    public Guid Id { get; set; }
    public string AccountAvatar { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string Link { get; set; } = null!;
    public string Content { get; set; } = null!;
    public List<string> Pictures { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class UpdatePostRequest
{
    public string Content { get; set; } = null!;
    public List<string> Pictures { get; set; } = [];
}

public class UpdatePostResponse
{
    public Guid Id { get; set; }
    public string AccountAvatar { get; set; } = null!;
    public string AccountName { get; set; } = null!;
    public string Link { get; set; } = null!;
    public string Content { get; set; } = null!;
    public List<string> PostPictures { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
}

public class PostPictureRequest
{
    public List<IFileStream> Pictures { get; set; } = null!;
}

public class UploadPostImageRequest
{
    public IFileStream[] Pictures { get; set; } = null!;
}


// public class PostComment
// {
//     public Guid Id { get; set; }
//     public string Username { get; set; } = null!;
//     public string DisplayName { get; set; } = null!;
//     public string Content { get; set; } = null!;
//     public int LikeCount { get; set; }
//     public int CommentCount { get; set; }
//     public bool IsLiked { get; set; }
//     public DateTime CreatedAt { get; set; }
// }
