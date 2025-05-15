using System;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.UsersService
{
    public class UserService : IUserService
    {
        private const string STATE_STORE = "chat-state-store";
        private const string USER_KEY_PREFIX = "user-";
        private const int USER_EXPIRY_MINUTES = 15;

        private readonly DaprClient _daprClient;
        private readonly ILogger<UserService> _logger;

        public UserService(DaprClient daprClient, ILogger<UserService> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public async Task<ChatUser?> GetUserAsync(Guid userId)
        {
            try
            {
                return await _daprClient.GetStateAsync<ChatUser>(STATE_STORE, $"{USER_KEY_PREFIX}{userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                return null;
            }
        }

        public async Task<UserJoinResultDto> JoinChatAsync(UserJoinDto joinRequest)
        {
            var user = new ChatUser(joinRequest.DisplayName);

            try
            {
                // Save the user with a TTL
                await _daprClient.SaveStateAsync(
                    STATE_STORE,
                    $"{USER_KEY_PREFIX}{user.Id}",
                    user,
                    metadata: new Dictionary<string, string>
                    {
                        { "ttlInSeconds", (USER_EXPIRY_MINUTES * 60).ToString() }
                    });

                return new UserJoinResultDto
                {
                    UserId = user.Id,
                    DisplayName = user.DisplayName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new user with display name {DisplayName}", joinRequest.DisplayName);
                throw;
            }
        }

        public async Task<ChatUser?> UpdateUserLastSeenAsync(Guid userId)
        {
            try
            {
                var user = await _daprClient.GetStateAsync<ChatUser>(STATE_STORE, $"{USER_KEY_PREFIX}{userId}");

                if (user != null)
                {
                    user.LastSeen = DateTime.UtcNow;

                    await _daprClient.SaveStateAsync(
                        STATE_STORE,
                        $"{USER_KEY_PREFIX}{userId}",
                        user,
                        metadata: new Dictionary<string, string>
                        {
                            { "ttlInSeconds", (USER_EXPIRY_MINUTES * 60).ToString() }
                        });
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last seen timestamp for user with ID {UserId}", userId);
                return null;
            }
        }
    }
}