using Application.Dtos;
using AutoMapper;
using BusinessObject.Entities;

public class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        Guid currentAccId = default;

        CreateMap<Message, LastMessageResponse>()
            .ForMember(
                dest => dest.IsOwner,
                opt => opt.MapFrom(src => src.SenderId == currentAccId)
            )
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(
                dest => dest.IsRead,
                opt => opt.MapFrom(src => src.SenderId == currentAccId || src.IsRead) // Sender always sees the message as read, while the receiver sees it as read only if IsRead is true
            )
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

        CreateMap<Message, MessagesResponse>()
            // .ForMember(
            //     dest => dest.IsOwner,
            //     opt => opt.MapFrom(src => src.SenderId == currentAccId)
            // )
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId));

        CreateMap<Conversation, ConversationsResponse>()
            .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.Id))
            .ForMember(
                dest => dest.Account,
                opt =>
                    opt.MapFrom(src => new AccountNameResponse
                    {
                        Id = src.Account1Id == currentAccId ? src.Account2Id : src.Account1Id,
                        DisplayName =
                            src.Account1Id == currentAccId
                                ? src.Account2.DisplayName
                                : src.Account1.DisplayName,
                        Username =
                            src.Account1Id == currentAccId
                                ? src.Account2.Username
                                : src.Account1.Username,
                        Avatar =
                            src.Account1Id == currentAccId
                                ? (src.Account2.Picture != null ? src.Account2.Picture.Link : "")
                                : (src.Account1.Picture != null ? src.Account1.Picture.Link : ""),
                        CreatedAt =
                            src.Account1Id == currentAccId
                                ? src.Account2.CreatedAt
                                : src.Account1.CreatedAt,
                        IsFollowing =
                            src.Account1Id == currentAccId
                                ? src.Account2.Followers.Any(f => f.FollowingId == currentAccId)
                                : src.Account1.Followers.Any(f => f.FollowingId == currentAccId),
                    })
            )
            .ForMember(
                dest => dest.LastMessage,
                opt =>
                    opt.MapFrom(src =>
                        src.Messages.Where(m => m.DeletedAt == null && !m.IsHidden)
                            .OrderByDescending(m => m.CreatedAt)
                            .FirstOrDefault()
                    )
            );
    }
}
