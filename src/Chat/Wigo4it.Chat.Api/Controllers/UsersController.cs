using Microsoft.AspNetCore.Mvc;
using Wigo4it.Chat.Core.Models;
using Wigo4it.Chat.UsersService;

namespace Wigo4it.Chat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("join")]
        public async Task<ActionResult<UserJoinResultDto>> JoinChat(UserJoinDto joinRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(joinRequest.DisplayName))
                {
                    return BadRequest("Display name is required");
                }

                var result = await _userService.JoinChatAsync(joinRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining chat with display name {DisplayName}", joinRequest.DisplayName);
                return StatusCode(500, "An error occurred while joining the chat");
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ChatUser>> GetUser(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserAsync(userId);

                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        [HttpPost("{userId}/ping")]
        public async Task<ActionResult> PingUser(Guid userId)
        {
            try
            {
                var user = await _userService.UpdateUserLastSeenAsync(userId);

                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last seen for user with ID {UserId}", userId);
                return StatusCode(500, "An error occurred while updating the user's last seen timestamp");
            }
        }
    }
}