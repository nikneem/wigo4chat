using System;
using System.Threading.Tasks;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.UsersService
{
    public interface IUserService
    {
        Task<ChatUser?> GetUserAsync(Guid userId);
        Task<UserJoinResultDto> JoinChatAsync(UserJoinDto joinRequest);
        Task<ChatUser?> UpdateUserLastSeenAsync(Guid userId);
    }
}