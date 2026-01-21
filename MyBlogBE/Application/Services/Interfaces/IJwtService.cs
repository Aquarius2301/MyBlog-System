using Application.Dtos;

namespace Application.Services.Interfaces;

/// <summary>
/// JWT Service Interface
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a signed JWT access token for the given account.
    /// </summary>
    /// <param name="account">The account for which the access token will be generated.</param>
    /// <returns>A JWT access token as a string.</returns>
    /// <remarks>
    /// The token includes claims for the account's Id and Username.
    /// Token expiration is based on the configured <see cref="AccessTokenDurationMinutes"/>.
    /// </remarks>
    string GenerateAccessToken(BusinessObject.Entities.Account account);

    /// <summary>
    /// Generates a random string to be used as a refresh token.
    /// </summary>
    /// <returns>A secure random string representing the refresh token.</returns>
    public string GenerateRefreshToken();

    /// <summary>
    /// Retrieves information about the currently authenticated user from the HTTP context.
    /// </summary>
    /// <returns>
    /// A <see cref="UserInfoResponse"/> containing the user's Id and Username.
    /// </returns>
    /// <remarks>
    /// Returns default values if the user is not authenticated or claims are missing.
    /// </remarks>
    public UserInfoResponse GetAccountInfo();
}
