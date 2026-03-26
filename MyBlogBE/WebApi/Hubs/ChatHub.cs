using Application.Services.Implementations;
using Application.Services.Interfaces;
using BusinessObject.Entities;
using DataAccess;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConversationService _service;

        public ChatHub(IConversationService service)
        {
            _service = service;
        }

        // public async Task SendMessage(Guid senderId, Guid receiverId, string message)
        // {
        //     await _service.SendMessage(receiverId, message);

        //     // 👇 chỉ gửi cho đúng người
        //     await Clients
        //         .User(receiverId.ToString())
        //         .SendAsync("ReceiveMessage", senderId.ToString(), message);

        //     // 👇 gửi lại cho chính mình
        //     await Clients
        //         .User(senderId.ToString())
        //         .SendAsync("ReceiveMessage", senderId.ToString(), message);
        // }
    }
}
