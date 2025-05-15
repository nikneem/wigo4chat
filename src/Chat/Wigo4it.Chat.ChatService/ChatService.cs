using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.ChatService
{
    public class ChatService : IChatService
    {
        private const string STATE_STORE = "chat-state-store";
        public const string PUB_SUB = "chat-pub-sub";
        private const string CHAT_HISTORY_KEY = "chat-history";
        public const string MESSAGE_TOPIC = "chat-messages";
        private const int MAX_CHAT_HISTORY = 50;

        private readonly DaprClient _daprClient;
        private readonly ILogger<ChatService> _logger;

        public ChatService(DaprClient daprClient, ILogger<ChatService> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync()
        {
            try
            {
                var chatHistory = await _daprClient.GetStateAsync<List<ChatMessage>>(STATE_STORE, CHAT_HISTORY_KEY);
                return chatHistory ?? new List<ChatMessage>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat history");
                return new List<ChatMessage>();
            }
        }

        public async Task<ChatMessage?> SendMessageAsync(Guid userId, string displayName, string message)
        {
            try
            {
                // Create a new message
                var chatMessage = new ChatMessage(userId, displayName, message);

                // Add message to history
                await AddMessageToHistoryAsync(chatMessage);

                // Publish to pub/sub for real-time updates
                await _daprClient.PublishEventAsync(PUB_SUB, MESSAGE_TOPIC, chatMessage);

                return chatMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message from user {UserId}", userId);
                return null;
            }
        }

        private async Task AddMessageToHistoryAsync(ChatMessage message)
        {
            // Get current history
            var history = await _daprClient.GetStateAsync<List<ChatMessage>>(STATE_STORE, CHAT_HISTORY_KEY)
                          ?? new List<ChatMessage>();

            // Add message to history
            history.Add(message);

            // Limit history size by removing oldest messages if needed
            if (history.Count > MAX_CHAT_HISTORY)
            {
                history.RemoveRange(0, history.Count - MAX_CHAT_HISTORY);
            }

            // Save updated history
            await _daprClient.SaveStateAsync(STATE_STORE, CHAT_HISTORY_KEY, history);
        }
    }
}