using Microsoft.AspNetCore.SignalR;
using Dapr;
using Dapr.Client;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.Api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinChat(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ChatRoom");
            _logger.LogInformation("User {UserId} connected to chat", userId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChatRoom");
            await base.OnDisconnectedAsync(exception);
        }

        [Topic("chat-pub-sub", "chat-messages")]
        public async Task BroadcastMessage(ChatMessage message)
        {
            await Clients.Group("ChatRoom").SendAsync("ReceiveMessage", message);
        }
    }
}