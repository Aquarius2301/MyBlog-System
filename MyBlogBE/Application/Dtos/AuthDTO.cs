namespace Application.Dtos;

public class AuthRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}

public class RefreshRequest
{
    public string Token { get; set; } = null!;
}

public class UserInfoResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string StatusType { get; set; } = null!;
}

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Email { get; set; } = null!;
}

public class ForgotPasswordResponse
{
    public string Identifier { get; set; } = null!;
}

public class ResetPasswordRequest
{
    public string ConfirmCode { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
