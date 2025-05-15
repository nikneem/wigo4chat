namespace Wigo4it.Chat.Client
{
    /// <summary>
    /// Options for configuring the Wigo4it Chat Client
    /// </summary>
    public class ChatClientOptions
    {
        /// <summary>
        /// The base URL of the chat service API
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// The SignalR hub URL for real-time chat
        /// </summary>
        public string HubUrl { get; set; } = string.Empty;
    }
}