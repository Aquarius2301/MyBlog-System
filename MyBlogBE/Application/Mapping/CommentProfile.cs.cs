using Application.Dtos;
using AutoMapper;
using BusinessObject.Entities;

namespace Application.Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        Guid currentAccId = default;

        CreateMap<Comment, GetCommentsResponse>()
            .ForMember(dest => dest.Commenter, opt => opt.MapFrom(src => src.Account))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Replies.Count))
            .ForMember(
                dest => dest.IsLiked,
                opt =>
                    opt.MapFrom(src =>
                        src.CommentLikes.Any(l =>
                            l.AccountId == currentAccId && l.CommentId == src.Id
                        )
                    )
            )
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.CommentLikes.Count))
            .ForMember(
                dest => dest.Pictures,
                opt => opt.MapFrom(src => src.Pictures.Select(pic => pic.Link).ToList())
            );

        CreateMap<Comment, PostLatestComment>()
            .ForMember(dest => dest.Commenter, opt => opt.MapFrom(src => src.Account));
    }
}
