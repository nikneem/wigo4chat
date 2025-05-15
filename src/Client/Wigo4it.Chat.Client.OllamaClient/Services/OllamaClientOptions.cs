using Wigo4it.Chat.Client;

namespace Wigo4it.Chat.Client.OllamaClient.Services;

public class OllamaClientOptions : ChatClientOptions
{
    /// <summary>
    /// The unique identifier for the Ollama bot
    /// </summary>
    public string UserId { get; set; } = "ollama-bot";

    /// <summary>
    /// The display name for the Ollama bot in the chat
    /// </summary>
    public string UserName { get; set; } = "OllamaBot";

    /// <summary>
    /// The chat service endpoint URL
    /// </summary>
    public string ChatServiceEndpoint { get; set; } = "http://localhost:5122";
}