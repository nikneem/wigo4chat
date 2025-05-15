using System;

namespace Wigo4it.Chat.Core.Models
{
    public class ChatUser
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; }

        public ChatUser()
        {
            LastSeen = DateTime.UtcNow;
        }

        public ChatUser(string displayName)
        {
            Id = Guid.NewGuid();
            DisplayName = displayName;
            LastSeen = DateTime.UtcNow;
        }
    }
}