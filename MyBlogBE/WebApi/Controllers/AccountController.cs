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
    private readonly IJwtService _jwtService;

    public AccountController(IAccountService service, IJwtService jwtService)
    {
        _service = service;
        _jwtService = jwtService;
    }

    [HttpGet("profile/me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = _jwtService.GetAccountInfo();

        var account = await _service.GetProfileByIdAsync(user.Id);

        return account == null
            ? throw new NotFoundException("NoAccount")
            : HandleResponse(Success(account));
    }

    [HttpGet("profile/{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var account = await _service.GetProfileByIdAsync(id);

        return account == null
            ? throw new NotFoundException("NoAccount")
            : HandleResponse(Success(account));
    }

    [HttpGet("profile/username/{username}")]
    public async Task<IActionResult> GetProfileByUsername(string username)
    {
        var user = _jwtService.GetAccountInfo();

        var account = await _service.GetProfileByUsernameAsync(username, user.Id);

        return account == null
            ? throw new NotFoundException("NoAccount")
            : HandleResponse(Success(account));
    }

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

    [HttpPut("profile/me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateAccountRequest request)
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.UpdateAccountAsync(user.Id, request);

        return res != null
            ? HandleResponse(Success(res))
            : throw new NotFoundException("NoAccount");
    }

    [HttpPut("profile/me/change-password")]
    public async Task<IActionResult> ChangeMyPassword([FromBody] UpdatePasswordRequest request)
    {
        var user = _jwtService.GetAccountInfo();

        var errors = new Dictionary<string, string>();

        // Check if the old password is correct
        if (
            string.IsNullOrWhiteSpace(request.OldPassword) // empty check
            || !await _service.IsPasswordCorrectAsync(user.Id, request.OldPassword) // correct check
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

        var res = await _service.ChangePasswordAsync(user.Id, request.NewPassword);

        return res
            ? HandleResponse(Success<object>(null, "PasswordChanged"))
            : BadRequest(new ApiResponse<object?>(400, "PasswordChangeFailed"));
    }

    [HttpPut("profile/me/change-avatar")]
    public async Task<IActionResult> ChangeMyAvatar([FromBody] ChangeAvatarRequest request)
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.ChangeAvatarAsync(user.Id, request.Picture);

        return res
            ? HandleResponse(Success<object>(null, "AvatarChanged"))
            : NotFound(new ApiResponse<object?>(404, "AvatarChangeFailed"));
    }

    [HttpPut("profile/me/self-remove")]
    public async Task<IActionResult> SelfRemoveMyAccount()
    {
        var user = _jwtService.GetAccountInfo();

        var res = await _service.SelfRemoveAccount(user.Id);

        // if (res != null)
        // {
        //     var content =
        //         $"{_lang.Get("SelfRemoveAccountEmail1")} {_settings.SelfRemoveDurationDays} {_lang.Get("SelfRemoveAccountEmail2")} ({res})";
        //     return ApiResponse.Success(content);
        // }

        return res != null
            ? HandleResponse(Success(res))
            : NotFound(new ApiResponse<object?>(404, "AccountNotFound"));
    }
}
