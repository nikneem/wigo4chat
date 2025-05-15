using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.Client
{
    /// <summary>
    /// Implementation of IChatClient to interact with the Wigo4it Chat service
    /// </summary>
    public class ChatClient : IChatClient
    {
        private readonly HttpClient _httpClient;
        private readonly ChatClientOptions _options;

        public ChatClient(HttpClient httpClient, IOptions<ChatClientOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            
            if (!string.IsNullOrEmpty(_options.BaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            }
        }

        /// <inheritdoc />
        public async Task<UserJoinResultDto> JoinChatAsync(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException("Display name is required", nameof(displayName));
            }

            var request = new UserJoinDto { DisplayName = displayName };
            var response = await _httpClient.PostAsJsonAsync("api/users/join", request);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<UserJoinResultDto>();
            return result ?? throw new InvalidOperationException("Failed to join chat");
        }

        /// <inheritdoc />
        public async Task<ChatUser?> GetUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            var response = await _httpClient.GetAsync($"api/users/{userId}");
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
            }
            
            return await response.Content.ReadFromJsonAsync<ChatUser>();
        }

        /// <inheritdoc />
        public async Task<bool> PingUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            var response = await _httpClient.PostAsync($"api/users/{userId}/ping", null);
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                
                response.EnsureSuccessStatusCode();
            }
            
            return true;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync()
        {
            var response = await _httpClient.GetAsync("api/chat/history");
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ChatMessage>>();
            return result ?? Enumerable.Empty<ChatMessage>();
        }

        /// <inheritdoc />
        public async Task<ChatMessage?> SendMessageAsync(Guid userId, string message)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be empty", nameof(message));
            }

            var request = new SendMessageDto
            {
                UserId = userId,
                Message = message
            };

            var response = await _httpClient.PostAsJsonAsync("api/chat/message", request);
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                response.EnsureSuccessStatusCode();
            }
            
            return await response.Content.ReadFromJsonAsync<ChatMessage>();
        }
    }
}