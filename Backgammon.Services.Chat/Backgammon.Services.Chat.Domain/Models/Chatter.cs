using System;
using System.Collections.Generic;

namespace Backgammon.Services.Chat.Domain.Models
{
    public class Chatter
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
