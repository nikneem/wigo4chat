using System;

namespace Wigo4it.Chat.Core.Models
{
    public class UserJoinResultDto
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}