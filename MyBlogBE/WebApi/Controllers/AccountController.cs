using Application.Dtos;
using Application.Exceptions;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;
using WebApi.Helpers;

namespace WebApi.Controllers;

[Authorize]
[Route("api/accounts")]
[ApiController]
[AuthorizeStatusAttribute(["active", "suspended"])]
public class AccountController : BaseController
{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get my profile information
    /// </summary>
    /// <returns>
    /// 200 - Returns profile of the account.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("profile/me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var account = await _service.GetProfileByIdAsync();

        return HandleResponse(Success(account));
    }

    /// <summary>
    /// Get profile information by account ID
    /// </summary>
    /// <param name="id">The unique identifier of the account</param>
    /// <returns>
    /// 200 - Returns profile of the account.
    /// 404 - Returns error if the account does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("profile/{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var account = await _service.GetProfileByIdAsync(id);

        return HandleResponse(Success(account));
    }

    /// <summary>
    /// Get profile information by username
    /// </summary>
    /// <param name="username">The username of the account</param>
    /// <returns>
    /// 200 - Returns profile of the account.
    /// 404 - Returns error if the account does not exist.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("profile/username/{username}")]
    public async Task<IActionResult> GetProfileByUsername(string username)
    {
        var account = await _service.GetProfileByUsernameAsync(username);

        return HandleResponse(Success(account));
    }

    /// <summary>
    /// Get accounts by name with pagination
    /// </summary>
    /// <param name="name">The name to search for</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>
    /// 200 - Returns accounts matching the name with pagination.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("")]
    public async Task<IActionResult> GetAccountName(
        [FromQuery] string name,
        [FromQuery] PaginationRequest pagination
    )
    {
        var res = await _service.GetAccountByNameAsync(
            name,
            pagination.Cursor,
            pagination.PageSize
        );

        return HandleResponse(
            Success(
                new PaginationResponse
                {
                    Items = res.Item1,
                    Cursor = res.Item2,
                    PageSize = pagination.PageSize,
                }
            )
        );
    }

    /// <summary>
    /// Update my profile information
    /// </summary>
    /// <param name="request">The update account request data</param>
    /// <returns>
    /// 200 - Returns the updated profile of the account.
    /// 400 - Returns error if the request data is invalid.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPut("profile/me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateAccountRequest request)
    {
        var errors = new Dictionary<string, string>();

        if (
            request.Username != null
            && !ValidationHelper.IsValidString(request.Username, true, 3, 20)
        )
            errors["username"] = "UsernameInvalid";

        if (
            request.DisplayName != null
            && !ValidationHelper.IsValidString(request.DisplayName, false, 3, 50)
        )
            errors["displayName"] = "DisplayNameLength";

        if (errors.Count > 0)
        {
            throw new BadRequestException("UpdateAccountFailed", errors);
        }
        var res = await _service.UpdateAccountAsync(request);

        return HandleResponse(Success(res));
    }

    /// <summary>
    /// Change my password
    /// </summary>
    /// <param name="request">The update password request data</param>
    /// <returns>
    /// 200 - Returns success if the password is changed successfully.
    /// 400 - Returns error if the old password is incorrect, the new password is not valid or same as the old password.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPut("profile/me/change-password")]
    public async Task<IActionResult> ChangeMyPassword([FromBody] UpdatePasswordRequest request)
    {
        var errors = new Dictionary<string, string>();

        // Check if the old password is correct
        if (
            string.IsNullOrWhiteSpace(request.OldPassword) // empty check
            || !await _service.IsPasswordCorrectAsync(request.OldPassword) // correct check
        )
        {
            errors["OldPassword"] = "OldPasswordIncorrect";
        }

        // Check if the new password is different from the old password
        if (request.OldPassword == request.NewPassword)
        {
            errors["NewPassword"] = "NewPasswordMustBeDifferent";
        }

        // Check if the new password is strong enough
        if (!ValidationHelper.IsStrongPassword(request.NewPassword))
        {
            errors["NewPassword"] = "PasswordRegister";
        }

        if (errors.Count > 0)
        {
            return BadRequest(new ApiResponse<object?>(400, "PasswordChangeFailed", errors));
        }

        await _service.ChangePasswordAsync(request.NewPassword);

        return HandleResponse(Success<object>(null, "PasswordChanged"));
    }

    /// <summary>
    /// Change my avatar
    /// </summary>
    /// <param name="request">The change avatar request data</param>
    /// <returns>
    /// 200 - Returns success if the avatar is changed successfully.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPut("profile/me/change-avatar")]
    public async Task<IActionResult> ChangeMyAvatar([FromBody] ChangeAvatarRequest request)
    {
        await _service.ChangeAvatarAsync(request.Picture);

        return HandleResponse(Success<object>(null, "AvatarChanged"));
    }

    /// <summary>
    /// Self remove my account
    /// </summary>
    /// <returns>
    /// 200 - Returns the time when the account will be permanently deleted.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPut("profile/me/self-remove")]
    public async Task<IActionResult> SelfRemoveMyAccount()
    {
        var res = await _service.SelfRemoveAccount();

        // if (res != null)
        // {
        //     var content =
        //         $"{_lang.Get("SelfRemoveAccountEmail1")} {_settings.SelfRemoveDurationDays} {_lang.Get("SelfRemoveAccountEmail2")} ({res})";
        //     return ApiResponse.Success(content);
        // }

        return HandleResponse(Success(res));
    }
}
