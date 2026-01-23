using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos;
using Application.Helpers;
using Application.Services.Interfaces;
using Application.Settings;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Implementations;

/// <summary>
/// Service class for generating JWT access tokens, refresh tokens,
/// and retrieving the current authenticated user's information.
/// </summary>
public class JwtService : IJwtService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly JwtSettings _settings;

    // /// <summary>
    // /// Gets the configured access token duration in minutes.
    // /// </summary>
    // public int AccessTokenDurationMinutes => _settings.AccessTokenDurationMinutes;

    // /// <summary>
    // /// Gets the configured refresh token duration in days.
    // /// </summary>
    // public int RefreshTokenDurationDays => _settings.RefreshTokenDurationDays;

    /// <summary>
    /// Initializes a new instance of the <see cref="IJwtService"/> class.
    /// </summary>
    /// <param name="options">Options containing JWT settings.</param>
    /// <param name="httpContextAccessor">Accessor to retrieve the current HTTP context.</param>
    public JwtService(IOptions<JwtSettings> options, IHttpContextAccessor httpContextAccessor)
    {
        _settings = options.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateAccessToken(Account account)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, account.Username),
            new Claim("Status", account.Status),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenDurationMinutes), //test
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return StringHelper.GenerateRandomString(64);
    }

    public UserInfoResponse GetAccountInfo()
    {
        // var user = _httpContextAccessor.HttpContext?.User;

        var user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as Account;

        var accountId = user!.Id;
        var accountUsername = user!.Username;
        var status = user!.Status;

        return new UserInfoResponse
        {
            Id = accountId,
            Username = accountUsername,
            StatusType = status,
        };
    }
}
