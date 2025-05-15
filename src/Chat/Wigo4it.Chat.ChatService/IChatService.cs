using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.ChatService
{
    public interface IChatService
    {
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync();
        Task<ChatMessage?> SendMessageAsync(Guid userId, string displayName, string message);
    }
}