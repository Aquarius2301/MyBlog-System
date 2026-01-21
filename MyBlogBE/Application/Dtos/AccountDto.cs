namespace Application.Dtos;

public class AccountResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Language { get; set; } = null!;
    public bool IsOwner { get; set; } = false;
    public DateOnly DateOfBirth { get; set; }
    public string AvatarUrl { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class AccountNameResponse
{
    public Guid Id { get; set; }
    public string Avatar { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsFollowing { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}

public class UpdateAccountRequest
{
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}

public class UpdateAccountResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdatePasswordRequest
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ChangeAvatarRequest
{
    public string Picture { get; set; } = null!;
}
