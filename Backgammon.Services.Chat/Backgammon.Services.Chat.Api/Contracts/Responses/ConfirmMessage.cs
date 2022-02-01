using System;

namespace Backgammon.Services.Chat.Api.Contracts.Responses
{
    public class ConfirmMessage
    {
        public Guid MessageId { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
