using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backgammon.Services.Chat.Infra.Data.Dtos
{
    [Table("Messages")]
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; }
        public bool IsRecived { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReceivedAt { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
    }
}
