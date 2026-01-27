using Application.Dtos;
using AutoMapper;
using BusinessObject.Entities;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        Guid currentAccId = default;

        CreateMap<Account, AccountNameResponse>()
            .ForMember(
                dest => dest.Avatar,
                opt => opt.MapFrom(src => src.Picture != null ? src.Picture.Link : "")
            )
            .ForMember(
                dest => dest.IsFollowing,
                opt =>
                    opt.MapFrom(src =>
                        src.Followers.Any(f =>
                            f.AccountId == currentAccId && f.FollowingId == src.Id
                        )
                    )
            );

        CreateMap<Account, AccountResponse>()
            .ForMember(
                dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.Picture != null ? src.Picture.Link : "")
            )
            .ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => src.Id == currentAccId));

        CreateMap<Account, RegisterResponse>();
    }
}
