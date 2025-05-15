using System;

namespace Wigo4it.Chat.Core.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }

        public ChatMessage()
        {
            Id = Guid.NewGuid();
            SentAt = DateTime.UtcNow;
        }

        public ChatMessage(Guid senderId, string senderName, string message)
        {
            Id = Guid.NewGuid();
            SenderId = senderId;
            SenderName = senderName;
            Message = message;
            SentAt = DateTime.UtcNow;
        }
    }
}