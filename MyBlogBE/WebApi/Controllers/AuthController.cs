using Application.Dtos;
using Application.Exceptions;
using Application.Services.Interfaces;
using Application.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.Attributes;
using WebApi.Helpers;

namespace WebApi.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _service;
    private readonly ILanguageService _lang;
    private readonly JwtSettings _settings;
    private readonly IJwtService _jwtService;

    public AuthController(
        IAuthService service,
        ILanguageService lang,
        IOptions<JwtSettings> settings,
        IJwtService jwtService
    )
    {
        _service = service;
        _lang = lang;
        _settings = settings.Value;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Authenticates a user with username and password.
    /// </summary>
    /// <param name="request">Login credentials containing username and password.</param>
    /// <returns>
    /// 200 - Returns authentication token if credentials are valid.
    /// 400 - Returns validation errors if input is invalid.
    /// 401 - Returns error if authentication fails.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        // Validate input
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            errors["username"] = _lang.Get("UsernameEmpty");
        }
        if (string.IsNullOrEmpty(request.Password))
        {
            errors["password"] = _lang.Get("PasswordEmpty");
        }
        if (errors.Count > 0)
        {
            throw new BadRequestException("BadRequest", errors);
        }

        // Authenticate user
        var res = await _service.GetAuthenticateAsync(request.Username, request.Password);

        // Save cookie on server
        if (res != null)
        {
            Response.Cookies.Append(
                "accessToken",
                res.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(
                        _settings.AccessTokenDurationMinutes
                    ),
                    Path = "/",
                }
            );
            Response.Cookies.Append(
                "refreshToken",
                res.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenDurationDays),
                    Path = "/",
                }
            );
            return HandleResponse(Success<object>(null, "Success"));
        }
        throw new UnauthorizedException("LoginFailed");
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">Registration data including username, email, password, display name, and date of birth.</param>
    /// <returns>
    /// 200 - Returns success message and send email to confirm if registration is successful.
    /// 400 - Returns validation errors if input is invalid or user already exists.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var errors = new Dictionary<string, string>();

        // Check validation
        if (!ValidationHelper.IsValidString(request.Username, true, 3, 20))
            errors["username"] = "UsernameInvalid";

        if (!ValidationHelper.IsValidString(request.DisplayName, false, 3, 50))
            errors["displayName"] = "DisplayNameLength";

        if (!ValidationHelper.IsStrongPassword(request.Password))
            errors["password"] = "PasswordInvalid";

        if (!ValidationHelper.IsValidEmail(request.Email))
            errors["email"] = "EmailInvalid";

        if (
            !ValidationHelper.IsValidDateOfBirth(request.DateOfBirth.ToDateTime(new TimeOnly(0, 0)))
        )
            errors["dateOfBirth"] = "DateOfBirthInvalid";
        if (errors.Count > 0)
        {
            throw new BadRequestException("BadRequest", errors);
        }

        // Check existence
        if (await _service.GetAccountByUsernameAsync(request.Username) != null)
        {
            errors["username"] = "UsernameExist";
        }
        if (await _service.GetAccountByEmailAsync(request.Email) != null)
        {
            errors["email"] = "EmailExist";
        }

        if (errors.Count > 0)
        {
            throw new BadRequestException("BadRequest", errors);
        }

        // Register account if no errors
        var res = await _service.RegisterAccountAsync(request);

        return HandleResponse(Success(res, "ConfirmEmail"));
    }

    /// <summary>
    /// Confirms user account registration or password reset via token.
    /// </summary>
    /// <param name="type">Type of confirmation: "register" or "forgotPassword".</param>
    /// <param name="token">Confirmation token sent to user's email.</param>
    /// <returns>
    /// 200 - Returns success message if token is valid.
    /// 400 - Returns error if type is invalid or token is invalid.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpGet("confirm")]
    // [AuthorizeStatusAttribute([BusinessObject.Enums.StatusType.InActive])]
    public async Task<IActionResult> ConfirmAccount(
        [FromQuery] string type,
        [FromQuery] string token
    )
    {
        if (type != "register" && type != "forgotPassword")
        {
            throw new BadRequestException("TypeError");
        }

        if (type == "register")
        {
            var res = await _service.ConfirmRegisterAccountAsync(token);
            return res
                ? HandleResponse(Success<object>(null, "Success"))
                : throw new BadRequestException("InvalidToken");
        }
        else
        {
            var res = await _service.ConfirmForgotPasswordAccountAsync(token);

            return !string.IsNullOrEmpty(res)
                ? HandleResponse(Success(res, "ForgotPasswordConfirmSuccessful"))
                : throw new BadRequestException("InvalidToken");
        }
    }

    /// <summary>
    /// Refreshes authentication token using refresh token.
    /// </summary>
    /// <param name="request">Request containing refresh token.</param>
    /// <returns>
    /// 200 - Returns new authentication token if refresh token is valid.
    /// 401 - Returns error if refresh token is invalid or expired.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPost("refresh")]
    // [AuthorizeStatusAttribute([
    //     BusinessObject.Enums.StatusType.Active,
    //     BusinessObject.Enums.StatusType.Suspended,
    // ])]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedException("InvalidToken");
        }
        var res = await _service.GetRefreshTokenAsync(refreshToken);

        if (res != null)
        {
            Response.Cookies.Append(
                "accessToken",
                res.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenDurationDays),
                    Path = "/",
                }
            );

            return HandleResponse(Success<object>(null, "Success"));
        }

        throw new UnauthorizedException("InvalidToken");
    }

    /// <summary>
    /// Initiates password reset process by sending confirmation email.
    /// </summary>
    /// <param name="request">Request containing username or email identifier.</param>
    /// <returns>
    /// 200 - Returns success message if account is found and email is sent.
    /// 400 - Returns error if account does not exist.
    /// 500 -  Returns error message if exception occurs.
    /// </returns>
    [HttpPost("forgot-password")]
    // [AuthorizeStatusAttribute([
    //     BusinessObject.Enums.StatusType.Active,
    //     BusinessObject.Enums.StatusType.Suspended,
    // ])]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordResponse request)
    {
        var res = await _service.ForgotPasswordAsync(request.Identifier);

        return res
            ? HandleResponse(Success(res, "ConfirmEmail"))
            : throw new BadRequestException("NoAccountFound");
    }

    /// <summary>
    /// Resets user password using confirmation code.
    /// </summary>
    /// <param name="request">Request containing confirmation code and new password.</param>
    /// <returns>
    /// 200 - Returns success message if password is reset successfully.
    /// 400 - Returns error if confirmation code or password is invalid.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ConfirmCode))
        {
            throw new BadRequestException("ConfirmCodeEmpty");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new BadRequestException("PasswordInvalid");
        }

        var res = await _service.ResetPasswordAsync(request.ConfirmCode, request.NewPassword);

        return res
            ? HandleResponse(Success<object>(null, "Success"))
            : throw new BadRequestException("InvalidToken");
    }

    /// <summary>
    /// Logs out the authenticated user by removing their refresh token.
    /// </summary>
    /// <returns>
    /// 200 - Returns success message if logout is successful.
    /// 401 - Returns error if user is not authenticated or logout fails.
    /// 500 - Returns error message if exception occurs.
    /// </returns>
    [Authorize]
    [HttpPost("logout")]
    [AuthorizeStatusAttribute(["active", "suspend"])]
    public async Task<IActionResult> Logout()
    {
        var user = _jwtService.GetAccountInfo();

        var ok = await _service.RemoveRefresh(user.Id);

        Response.Cookies.Delete(
            "accessToken",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
            }
        );

        Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
            }
        );

        return ok
            ? HandleResponse(Success<object>(null, "Success"))
            : throw new UnauthorizedException("InvalidToken");
    }
}
