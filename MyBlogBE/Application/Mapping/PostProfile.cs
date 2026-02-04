using Application.Dtos;
using AutoMapper;
using BusinessObject.Entities;

namespace Application.Mapping;

public class PostProfile : Profile
{
    public PostProfile()
    {
        Guid currentAccId = default;

        CreateMap<Post, GetPostDetailResponse>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Account))
            .ForMember(
                dest => dest.CommentCount,
                opt =>
                    opt.MapFrom(src =>
                        src.Comments.Count(c =>
                            // Parent comment has not deleted and
                            (c.ParentComment == null && c.DeletedAt == null)
                            ||
                            // Child comment has not deleted and parent also has not deleted
                            (
                                c.ParentComment != null
                                && c.DeletedAt == null
                                && c.ParentComment.DeletedAt == null
                            )
                        )
                    )
            )
            .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.PostLikes.Count()))
            .ForMember(
                dest => dest.IsLiked,
                opt => opt.MapFrom(src => src.PostLikes.Any(pl => pl.Account.Id == currentAccId))
            )
            .ForMember(
                dest => dest.IsOwner,
                opt => opt.MapFrom(src => currentAccId == src.Account.Id)
            )
            .ForMember(
                dest => dest.PostPictures,
                opt => opt.MapFrom(src => src.Pictures.Select(pic => pic.Link))
            );

        CreateMap<Post, GetPostsResponse>()
            .IncludeBase<Post, GetPostDetailResponse>()
            .ForMember(
                dest => dest.LatestComment,
                opt =>
                    opt.MapFrom(src =>
                        src.Comments.Where(x => x.DeletedAt == null && x.ParentCommentId == null)
                            .OrderByDescending(c => c.CreatedAt)
                            .FirstOrDefault()
                    )
            );
    }
}
