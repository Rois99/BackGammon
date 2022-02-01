using System;

namespace Backgammon.Services.Chat.Api.Contracts.Requests
{
    public class MessageRequest
    {
        public string MessageBody { get; set; }
        public Guid RecipientId { get; set; }
        public DateTime SentAt { get; set; }
    }
}
