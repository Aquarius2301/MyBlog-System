using Application.Dtos;
using Application.Exceptions;
using Application.Helpers;
using Application.Services.Interfaces;
using Application.Settings;
using AutoMapper;
using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccess.Extensions;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly BaseSettings _settings;
    private readonly IMapper _mapper;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IEmailService emailService,
        IOptions<BaseSettings> options,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _emailService = emailService;
        _settings = options.Value;
        _mapper = mapper;
    }

    // public Task<Account> GetAccountByNameOrEmailAsync(string identifier)
    // {
    //     var query =
    //         _unitOfWork
    //             .Accounts.GetQuery()
    //             .WhereDeletedIsNull()
    //             .WhereUsernameOrEmail(identifier)
    //             .FirstAsync()
    //         ?? throw new Exception("NoAccount");

    //     return query;
    // }

    public Task<Account?> GetAccountByUsernameAsync(string username)
    {
        var account = _unitOfWork
            .Accounts.ReadOnly()
            .WhereUsername(username)
            .WhereDeletedIsNull()
            .FirstOrDefaultAsync();

        return account;
    }

    public Task<Account?> GetAccountByEmailAsync(string email)
    {
        var account = _unitOfWork
            .Accounts.ReadOnly()
            .WhereEmail(email)
            .WhereDeletedIsNull()
            .FirstOrDefaultAsync();

        return account;
    }

    public async Task<AuthResponse> GetAuthenticateAsync(string username, string password)
    {
        var account = await _unitOfWork
            .Accounts.GetQuery()
            .WhereUsername(username)
            .WhereDeletedIsNull()
            .FirstOrDefaultAsync();

        if (
            account == null
            || !PasswordHasherHelper.VerifyPassword(password, account.HashedPassword)
        )
        {
            throw new NotFoundException("NoAccount");
        }

        account.AccessToken = _jwtService.GenerateAccessToken(account);
        account.RefreshToken = _jwtService.GenerateRefreshToken();
        account.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            _settings.JwtSettings.RefreshTokenDurationDays
        );
        account.SelfRemoveTime = null; // cancel self-removal if user logs in again

        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = account.AccessToken,
            RefreshToken = account.RefreshToken,
        };
    }

    public async Task<AuthResponse> GetRefreshTokenAsync(string refreshToken)
    {
        var account =
            await _unitOfWork
                .Accounts.GetQuery()
                .WhereRefreshToken(refreshToken)
                .WhereDeletedIsNull()
                .FirstOrDefaultAsync()
            ?? throw new UnauthorizedException("NoAccount");

        // if (account != null)
        // {
        if (
            account.RefreshToken != null
            && account.RefreshTokenExpiryTime != null
            && account.RefreshTokenExpiryTime >= DateTime.UtcNow
        )
        {
            account.AccessToken = _jwtService.GenerateAccessToken(account);

            await _unitOfWork.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = account.AccessToken,
                RefreshToken = account.RefreshToken,
            };
        }
        else
        {
            account.AccessToken = null;
            account.RefreshToken = null;
            account.RefreshTokenExpiryTime = null;

            await _unitOfWork.SaveChangesAsync();

            throw new UnauthorizedException("Unauthorized");
        }
        // }

        // return null;
    }

    public async Task RemoveRefreshAsync()
    {
        var accountId = _jwtService.GetAccountInfo().Id;

        var account = await _unitOfWork.Accounts.GetQuery().WhereId(accountId).FirstAsync();

        account.AccessToken = null;
        account.RefreshToken = null;
        account.RefreshTokenExpiryTime = null;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<RegisterResponse> RegisterAccountAsync(RegisterRequest request)
    {
        var tokenLength = _settings.TokenLength;
        var tokenTimeout = _settings.TokenExpiryMinutes;
        var confirmCode = StringHelper.GenerateRandomString(tokenLength, true);
        var currentTime = DateTime.UtcNow;

        await _emailService.SendRegisterEmailAsync(request.Email, request.Username, confirmCode);

        var newAccount = new Account
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            DisplayName = request.DisplayName,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            EmailVerifiedCode = confirmCode,
            VerificationType = VerificationType.Register,
            EmailVerifiedCodeExpiry = currentTime.AddMinutes(tokenTimeout),
            HashedPassword = PasswordHasherHelper.HashPassword(request.Password),
            Status = StatusType.Inactive.Code,
            CreatedAt = currentTime,
        };

        _unitOfWork.Accounts.Add(newAccount);
        await _unitOfWork.SaveChangesAsync();

        var res = _mapper.Map<RegisterResponse>(newAccount);

        return res;
        // return new RegisterResponse
        // {
        //     Id = newAccount.Id,
        //     Username = newAccount.Username,
        //     DisplayName = newAccount.DisplayName,
        //     DateOfBirth = newAccount.DateOfBirth,
        // };
    }

    private async Task<Account?> GetAccountByEmailVerifiedCodeAsync(
        string code,
        VerificationType type
    )
    {
        var account = await _unitOfWork
            .Accounts.GetQuery()
            .WhereEmailVerifiedCode(code)
            .WhereVerificationType(type)
            .FirstOrDefaultAsync();

        return account;
    }

    public async Task ConfirmRegisterAccountAsync(string confirmCode)
    {
        var account =
            await GetAccountByEmailVerifiedCodeAsync(confirmCode, VerificationType.Register)
            ?? throw new BadRequestException("InvalidToken");

        if (DateTime.UtcNow <= account.EmailVerifiedCodeExpiry)
        {
            account.EmailVerifiedCode = null;
            account.VerificationType = null;
            account.EmailVerifiedCodeExpiry = null;
            account.Status = StatusType.Active.Code;

            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new BadRequestException("InvalidToken");
        }
    }

    public async Task<string> ConfirmForgotPasswordAccountAsync(string confirmCode)
    {
        var account =
            await GetAccountByEmailVerifiedCodeAsync(confirmCode, VerificationType.ForgotPassword)
            ?? throw new BadRequestException("InvalidToken");

        if (DateTime.UtcNow <= account.EmailVerifiedCodeExpiry)
        {
            account.VerificationType = VerificationType.ChangePassword;
            account.EmailVerifiedCodeExpiry = null;

            await _unitOfWork.SaveChangesAsync();
            return confirmCode;
        }

        throw new BadRequestException("InvalidToken");
    }

    public async Task ForgotPasswordAsync(string identifier)
    {
        var tokenTimeout = _settings.TokenExpiryMinutes;
        var account =
            await _unitOfWork
                .Accounts.GetQuery()
                .WhereDeletedIsNull()
                .WhereUsernameOrEmail(identifier)
                .FirstOrDefaultAsync()
            ?? throw new BadRequestException("NoAccount");

        if (account != null)
        {
            var confirmCode = StringHelper.GenerateRandomString(_settings.TokenLength);
            await _emailService.SendForgotPasswordEmailAsync(
                account.Email,
                account.Username,
                confirmCode
            );

            account.EmailVerifiedCode = confirmCode;
            account.VerificationType = VerificationType.ForgotPassword;
            account.EmailVerifiedCodeExpiry = DateTime.UtcNow.AddMinutes(tokenTimeout);

            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ResetPasswordAsync(string confirmCode, string newPassowrd)
    {
        var account =
            await GetAccountByEmailVerifiedCodeAsync(confirmCode, VerificationType.ChangePassword)
            ?? throw new BadRequestException("InvalidToken");

        if (account != null)
        {
            account.VerificationType = null;
            account.EmailVerifiedCodeExpiry = null;
            account.EmailVerifiedCode = null;
            account.HashedPassword = PasswordHasherHelper.HashPassword(newPassowrd);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
