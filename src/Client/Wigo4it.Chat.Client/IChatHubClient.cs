using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.Client
{
    /// <summary>
    /// Interface for the Chat SignalR hub client that provides real-time chat functionality
    /// </summary>
    public interface IChatHubClient
    {
        /// <summary>
        /// Connect to the chat hub
        /// </summary>
        /// <returns>Task representing the connection operation</returns>
        Task ConnectAsync();

        /// <summary>
        /// Disconnect from the chat hub
        /// </summary>
        /// <returns>Task representing the disconnection operation</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Event that fires when a new message is received
        /// </summary>
        event Action<ChatMessage> OnMessageReceived;

        /// <summary>
        /// Event that fires when a user joins the chat
        /// </summary>
        event Action<ChatUser> OnUserJoined;

        /// <summary>
        /// Event that fires when a user leaves the chat
        /// </summary>
        event Action<Guid> OnUserLeft;
    }
}