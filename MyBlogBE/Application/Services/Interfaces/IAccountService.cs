using Application.Dtos;

namespace Application.Services.Interfaces;

/// <summary>
/// Account Service Interface
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Change or add account avatar.
    /// </summary>
    /// <param name="avatarFile">The new avatar file.</param>
    /// <returns>A task that represents the asynchronous change avatar operation.</returns>
    /// <exception cref="NotFoundException">Thrown when the account does not exist.</exception>
    Task ChangeAvatarAsync(string avatarFile);

    /// <summary>
    /// Change account password.
    /// </summary>
    /// <param name="password">The new password.</param>
    /// <returns>A task that represents the asynchronous change password operation.</returns>
    Task ChangePasswordAsync(string password);

    /// <summary>
    /// Get accounts by name with pagination.
    /// </summary>
    /// <param name="name">The name to search for (username or display name).</param>
    /// <param name="cursor">Timestamp of the last loaded child comment (used for pagination).</param>
    /// <param name="pageSize">The number of items to return per page.</param>
    /// <returns> A list of <see cref="AccountNameResponse"/> with the matching name.</returns>
    Task<(List<AccountNameResponse>, DateTime?)> GetAccountByNameAsync(
        string name,
        DateTime? cursor,
        int pageSize
    );

    /// <summary>
    /// Get profile information by account ID.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account. If not provided, the current account's ID will be used.</param>
    /// <returns> An <see cref="AccountResponse"/> contains profile information of the account if found; </returns>
    /// <exception cref="NotFoundException">Thrown when the account does not exist.</exception>
    Task<AccountResponse> GetProfileByIdAsync(Guid accountId = default);

    /// <summary>
    /// Get profile information by username.
    /// </summary>
    /// <param name="username">The username of the account.</param>
    /// <returns> An <see cref="AccountResponse"/> contains profile information of the account if found; </returns>
    /// <exception cref="NotFoundException">Thrown when the account does not exist.</exception>
    Task<AccountResponse> GetProfileByUsernameAsync(string username);

    /// <summary>
    /// Update account information
    /// </summary>
    /// <param name="request">The update account request data.</param>
    /// <returns> An <see cref="UpdateAccountResponse"/> contains updated account information if successful;</returns>
    /// <exception cref="BadRequestException">Thrown when the username already exists.</exception>
    Task<AccountResponse> UpdateAccountAsync(UpdateAccountRequest request);

    /// <summary>
    /// Check if the provided password is correct for the account
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <returns>True if the password is correct; otherwise, false.</returns>
    Task<bool> IsPasswordCorrectAsync(string password);

    /// <summary>
    /// Schedule account for self-removal
    /// </summary>
    /// <returns>The scheduled self-removal time if the account exists</returns>
    Task<DateTime> SelfRemoveAccount();
}
