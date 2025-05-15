using Microsoft.AspNetCore.Mvc;
using Wigo4it.Chat.ChatService;
using Wigo4it.Chat.Core.Models;
using Wigo4it.Chat.UsersService;

namespace Wigo4it.Chat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, IUserService userService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatHistory()
        {
            try
            {
                var history = await _chatService.GetChatHistoryAsync();
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat history");
                return StatusCode(500, "An error occurred while retrieving chat history");
            }
        }

        [HttpPost("message")]
        public async Task<ActionResult<ChatMessage>> SendMessage(SendMessageDto messageDto)
        {
            try
            {
                // Validate if user exists
                var user = await _userService.GetUserAsync(messageDto.UserId);
                if (user == null)
                {
                    return NotFound($"User with ID {messageDto.UserId} not found");
                }

                // Update user's last seen time
                await _userService.UpdateUserLastSeenAsync(messageDto.UserId);

                // Send message
                var message = await _chatService.SendMessageAsync(user.Id, user.DisplayName, messageDto.Message);

                if (message == null)
                {
                    return StatusCode(500, "Failed to send message");
                }

                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message for user {UserId}", messageDto.UserId);
                return StatusCode(500, "An error occurred while sending the message");
            }
        }


    }
}