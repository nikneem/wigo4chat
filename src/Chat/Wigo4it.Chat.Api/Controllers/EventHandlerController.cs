using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Wigo4it.Chat.Api.Hubs;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.Api.Controllers;




[Route("api/[controller]")]
[ApiController]
public class EventHandlerController(IHubContext<ChatHub> chatHub) : ControllerBase
{

    [HttpPost("chat-message-sent-handler")]
    [Topic(ChatService.ChatService.PUB_SUB, ChatService.ChatService.MESSAGE_TOPIC)]
    public async Task<IActionResult> HandleMessage([FromBody] ChatMessage message)
    {
        // Process the message
        // For example, save it to a database or send it to a chat room
        await chatHub.Clients.All.SendAsync("ReceiveMessage", message, HttpContext.RequestAborted);
        return Ok();
    }


}