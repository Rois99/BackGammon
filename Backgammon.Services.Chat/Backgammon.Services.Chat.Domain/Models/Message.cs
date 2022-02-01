using System;

namespace Backgammon.Services.Chat.Domain.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string MessageBody { get; set; }
        public bool IsReceived { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
