using System;

namespace Wigo4it.Chat.Core.Models
{
    public class SendMessageDto
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}