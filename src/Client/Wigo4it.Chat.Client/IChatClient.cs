using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.Client
{
    /// <summary>
    /// Interface for the Chat client that provides methods to interact with the Wigo4it Chat service
    /// </summary>
    public interface IChatClient
    {
        /// <summary>
        /// Join the chat with a display name
        /// </summary>
        /// <param name="displayName">Display name of the user</param>
        /// <returns>Result of the join operation including the user ID</returns>
        Task<UserJoinResultDto> JoinChatAsync(string displayName);

        /// <summary>
        /// Get user details by ID
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>User details or null if not found</returns>
        Task<ChatUser?> GetUserAsync(Guid userId);

        /// <summary>
        /// Update user's last seen timestamp (ping)
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> PingUserAsync(Guid userId);

        /// <summary>
        /// Get the chat history
        /// </summary>
        /// <returns>Collection of chat messages</returns>
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync();

        /// <summary>
        /// Send a message to the chat
        /// </summary>
        /// <param name="userId">ID of the sending user</param>
        /// <param name="message">Message content</param>
        /// <returns>The sent message if successful, null otherwise</returns>
        Task<ChatMessage?> SendMessageAsync(Guid userId, string message);
    }
}