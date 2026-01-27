using Application.Dtos;
using BusinessObject.Entities;

namespace Application.Services.Interfaces;

/// <summary>
/// Authentication Service Interface
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Gets an account by its username.
    /// </summary>
    /// <param name="username">The username of the account.</param>
    /// <returns>
    /// An <see cref="Account"/> with the specified username if found; otherwise, null.
    /// </returns>
    Task<Account?> GetAccountByUsernameAsync(string username);

    /// <summary>
    /// Gets an account by its email.
    /// </summary>
    /// <param name="email">The email of the account.</param>
    /// <returns>
    /// An <see cref="Account"/> with the specified email if found; otherwise, null.
    /// </returns>
    Task<Account?> GetAccountByEmailAsync(string email);

    /// <summary>
    /// Authenticates a user with the given username and password.
    /// </summary>
    /// <param name="username">The username of the account.</param>
    /// <param name="password">The password of the account.</param>
    /// <returns>
    /// <para>An <see cref="AuthResponse"/> object if authentication is successful; otherwise, null.</para>
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when no account with the specified username is found or wrong password.</exception>
    Task<AuthResponse> GetAuthenticateAsync(string username, string password);

    /// <summary>
    /// Generates a new access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token issued to the user.</param>
    /// <returns>
    /// <para>An <see cref="AuthResponse"/> object with a new access token if valid; otherwise, null.</para>
    /// </returns>
    /// <exception cref="UnauthorizedException">Thrown when the refresh token does not match any account or expired.</exception>
    Task<AuthResponse> GetRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Removes the refresh token associated with a user, effectively logging them out.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    Task RemoveRefreshAsync();

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The registration request containing username, password, email, and display name.</param>
    /// <returns>
    /// <para>A <see cref="RegisterResponse"/> object containing the newly created account details.</para>
    /// </returns>
    Task<RegisterResponse> RegisterAccountAsync(RegisterRequest request);

    /// <summary>
    /// Confirms a newly registered account using the provided confirmation code.
    /// </summary>
    /// <param name="confirmCode">The confirmation token sent to the user's email.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="BadRequestException">Thrown when the confirmation code is invalid or expired.</exception>
    Task ConfirmRegisterAccountAsync(string confirmCode);

    /// <summary>
    /// Confirms a password reset request using the provided confirmation code.
    /// </summary>
    /// <param name="confirmCode">The confirmation token sent to the user's email for password reset.</param>
    /// <returns>
    /// The new password or success message if confirmation succeeds; otherwise, null.
    /// </returns>
    /// <exception cref="BadRequestException">Thrown when the confirmation code is invalid or expired.</exception>
    Task<string> ConfirmForgotPasswordAccountAsync(string confirmCode);

    /// <summary>
    /// Initiates a forgot password process for the given identifier.
    /// </summary>
    /// <param name="identifier">The username or email of the account requesting password reset.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="BadRequestException">Thrown when no account with the specified identifier is found.</exception>
    Task ForgotPasswordAsync(string identifier);

    /// <summary>
    /// Resets the user's password using a confirmation code and new password.
    /// </summary>
    /// <param name="confirmCode">The confirmation token sent to the user's email.</param>
    /// <param name="newPassowrd">The new password to set for the account.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="BadRequestException">Thrown when the confirmation code is invalid or expired.</exception>
    Task ResetPasswordAsync(string confirmCode, string newPassowrd);
}
