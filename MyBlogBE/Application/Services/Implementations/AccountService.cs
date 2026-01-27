using Application.Dtos;
using Application.Exceptions;
using Application.Helpers;
using Application.Services.Interfaces;
using Application.Settings;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccess.Extensions;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BaseSettings _baseSettings;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;
    private Guid AccountId => _jwtService.GetAccountInfo().Id;

    public AccountService(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IOptions<BaseSettings> options,
        IMapper mapper,
        IJwtService jwtService
    )
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _baseSettings = options.Value;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    public async Task<(List<AccountNameResponse>, DateTime?)> GetAccountByNameAsync(
        string name,
        DateTime? cursor,
        int pageSize
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            return ([], null);

        var query = _unitOfWork
            .Accounts.ReadOnly()
            .WhereContainsDisplaynameOrUsername(name)
            .WhereIf(cursor.HasValue, a => a.WhereCursorLessThan(cursor!.Value))
            .OrderByDescending(a => a.CreatedAt)
            .Take(pageSize + 1);

        var accounts = await query
            // .Select(a => new AccountNameResponse
            // {
            //     Id = a.Id,
            //     Username = a.Username,
            //     DisplayName = a.DisplayName,
            //     Avatar = a.Picture != null ? a.Picture.Link : string.Empty,
            //     CreatedAt = a.CreatedAt,
            // })
            .ProjectTo<AccountNameResponse>(
                _mapper.ConfigurationProvider,
                new { currentAccId = AccountId } // Has not following info here (fix later)
            )
            .ToListAsync();

        var hasMore = accounts.Count > pageSize;
        var nextCursor = hasMore ? accounts.Last().CreatedAt : (DateTime?)null;
        var result = accounts.Take(pageSize).ToList();

        return (result, nextCursor);
    }

    public async Task<AccountResponse> GetProfileByIdAsync(Guid accountId = default)
    {
        if (accountId == default)
        {
            accountId = AccountId;
        }

        var account =
            await _unitOfWork
                .Accounts.ReadOnly()
                .WhereId(accountId)
                // .Select(a => new AccountResponse
                // {
                //     Id = a.Id,
                //     Username = a.Username,
                //     Email = a.Email,
                //     IsOwner = a.Id == accountId,
                //     Language = a.Language.ToString(),
                //     DisplayName = a.DisplayName,
                //     DateOfBirth = a.DateOfBirth,
                //     AvatarUrl = a.Picture != null ? a.Picture.Link : "",
                //     Status = a.Status.ToString(),
                //     CreatedAt = a.CreatedAt,
                // })
                .ProjectTo<AccountResponse>(
                    _mapper.ConfigurationProvider,
                    new { currentAccId = AccountId }
                )
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException("NoAccount");

        return account;
    }

    public async Task<AccountResponse> GetProfileByUsernameAsync(string username)
    {
        var account =
            await _unitOfWork
                .Accounts.ReadOnly()
                .WhereUsername(username)
                // .Select(a => new AccountResponse
                // {
                //     Id = a.Id,
                //     Username = a.Username,
                //     Email = a.Email,
                //     IsOwner = a.Id == userId,
                //     DisplayName = a.DisplayName,
                //     DateOfBirth = a.DateOfBirth,
                //     AvatarUrl = a.Picture != null ? a.Picture.Link : "",
                //     Status = a.Status.ToString(),
                //     CreatedAt = a.CreatedAt,
                // })
                .ProjectTo<AccountResponse>(
                    _mapper.ConfigurationProvider,
                    new { currentAccId = AccountId }
                )
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException("NoAccount");

        return account;
    }

    public async Task<AccountResponse> UpdateAccountAsync(UpdateAccountRequest request)
    {
        var account = await _unitOfWork
            .Accounts.GetQuery()
            .WhereId(AccountId)
            .WhereDeletedIsNull()
            .FirstAsync();
        // if (account == null)
        // {
        //     return null;
        // }

        if (request.Username != null)
            if (await _unitOfWork.Accounts.ReadOnly().WhereUsername(request.Username).AnyAsync())
                throw new BadRequestException("UsernameExists");

        var updateTime = DateTime.UtcNow;
        account.Username = request.Username ?? account.Username;
        account.DisplayName = request.DisplayName ?? account.DisplayName;
        account.DateOfBirth = request.DateOfBirth ?? account.DateOfBirth;
        account.UpdatedAt = updateTime;

        await _unitOfWork.SaveChangesAsync();

        var res = _mapper.Map<AccountResponse>(account);

        return res;
        // return new AccountResponse
        // {
        //     Id = account.Id,
        //     Username = account.Username,
        //     DisplayName = account.DisplayName,
        //     DateOfBirth = account.DateOfBirth,
        //     AvatarUrl = account.Picture != null ? account.Picture.Link : "",
        //     IsOwner = true,
        //     CreatedAt = account.CreatedAt,
        //     Email = account.Email,
        //     Status = account.Status.ToString(),
        //     Language = account.Language.ToString(),
        // };
    }

    public async Task ChangePasswordAsync(string password)
    {
        var account = await _unitOfWork
            .Accounts.GetQuery()
            .WhereId(AccountId)
            .WhereDeletedIsNull()
            .FirstAsync();

        // if (account == null)
        // {
        //     return false;
        // }

        account.HashedPassword = PasswordHasherHelper.HashPassword(password);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsPasswordCorrectAsync(string password)
    {
        var account = await _unitOfWork
            .Accounts.ReadOnly()
            .WhereId(AccountId)
            .WhereDeletedIsNull()
            .FirstAsync();

        return PasswordHasherHelper.VerifyPassword(password, account.HashedPassword);
    }

    public async Task ChangeAvatarAsync(string avatarFile)
    {
        //Check if account exists
        var account = await _unitOfWork
            .Accounts.ReadOnly()
            .Include(a => a.Picture)
            .WhereId(AccountId)
            .WhereDeletedIsNull()
            .FirstOrDefaultAsync();

        // if (account == null)
        // {
        //     return false;
        // }

        var picture = await _unitOfWork
            .Pictures.GetQuery()
            .WhereLink(avatarFile)
            .ExecuteUpdateAsync(p => p.SetProperty(p => p.AccountId, AccountId));

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<DateTime> SelfRemoveAccount()
    {
        var account = await _unitOfWork
            .Accounts.GetQuery()
            .WhereId(AccountId)
            .WhereDeletedIsNull()
            .WhereSelfRemoveTimeIsNull()
            .FirstAsync();

        // if (account == null)
        // {
        //     return null;
        // }
        var selfRemoveTime = DateTime.UtcNow.AddDays(_baseSettings.SelfRemoveDurationDays);

        account.SelfRemoveTime = selfRemoveTime;

        await _emailService.SendAccountRemovalEmailAsync(account.Email);

        await _unitOfWork.SaveChangesAsync();

        return selfRemoveTime;
    }
}
