using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backgammon.Services.Chat.Infra.Data.Dtos
{
    [Table("Connections")]
    public class ConnectionDto
    {
        public string Id { get; set; }
        public bool IsClosed { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }

        public Guid ChatterId { get; set; }
        public virtual ChatterDto Chatter { get; set; }
    }
}
