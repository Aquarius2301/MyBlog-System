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
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <param name="avatarFile">The new avatar file.</param>
    /// <returns> A <see cref="ImageDto"/> contains image data if the avatar was successfully changed;
    /// otherwise, null.</returns>
    Task<bool> ChangeAvatarAsync(Guid accountId, string avatarFile);

    /// <summary>
    /// Change account password.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <param name="password">The new password.</param>
    /// <returns>True if the password was successfully changed; otherwise, false.</returns>
    Task<bool> ChangePasswordAsync(Guid accountId, string password);

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
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <returns> An <see cref="AccountResponse"/> contains profile information of the account if found;
    /// otherwise, null.</returns>
    Task<AccountResponse?> GetProfileByIdAsync(Guid accountId);
    Task<AccountResponse?> GetProfileByUsernameAsync(string username, Guid userId);

    /// <summary>
    /// Update account information
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <param name="request">The update account request data.</param>
    /// <returns> An <see cref="UpdateAccountResponse"/> contains updated account information if successful;
    /// otherwise, null.</returns>
    Task<AccountResponse?> UpdateAccountAsync(Guid accountId, UpdateAccountRequest request);

    /// <summary>
    /// Check if the provided password is correct for the account
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <param name="password">The password to verify.</param>
    /// <returns>True if the password is correct; otherwise, false.</returns>
    Task<bool> IsPasswordCorrectAsync(Guid accountId, string password);

    /// <summary>
    /// Schedule account for self-removal
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <returns>The scheduled self-removal time if the account exists; otherwise, null.</returns>
    Task<DateTime?> SelfRemoveAccount(Guid accountId);
}
