using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.Client.OllamaClient.Services;

public class ChatMessageHandler : IChatMessageHandler
{
    private readonly IChatClient _chatClient;
    private readonly IOllamaService _ollamaService;
    private readonly ILogger<ChatMessageHandler> _logger;
    private readonly OllamaClientOptions _options;
    private const string OLLAMA_PREFIX = "ollama";

    public ChatMessageHandler(
        IChatClient chatClient,
        IOllamaService ollamaService,
        ILogger<ChatMessageHandler> logger,
        IOptions<OllamaClientOptions> options)
    {
        _chatClient = chatClient;
        _ollamaService = ollamaService;
        _logger = logger;
        _options = options.Value;
    }

    public async Task HandleMessageAsync(ChatMessage message)
    {
        // Skip our own messages to avoid infinite loops
        if (message.SenderName == _options.UserName)
        {
            return;
        }

        var content = message.Message.Trim();

        // Check if message starts with "ollama" (case insensitive)
        if (content.StartsWith(OLLAMA_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Received ollama request from {User}: {Content}", message.SenderName, content);

            // Extract the question (everything after "ollama ")
            var question = content.Substring(OLLAMA_PREFIX.Length).Trim();
            
            if (string.IsNullOrWhiteSpace(question))
            {
                await SendResponseAsync("Please provide a question after 'ollama'.");
                return;
            }

            try
            {
                // Get response from Ollama
                var response = await _ollamaService.GetCompletionAsync(question);
                
                // Send response back to chat
                await SendResponseAsync($"@{message.SenderName} asked: {question}\n\n{response}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ollama request");
                await SendResponseAsync($"Sorry, I encountered an error while processing your request: {ex.Message}");
            }
        }
    }

    private async Task SendResponseAsync(string message)
    {
        await _chatClient.SendMessageAsync(Guid.Parse(_options.UserId), message);
    }
}

public interface IChatMessageHandler
{
    Task HandleMessageAsync(ChatMessage message);
}