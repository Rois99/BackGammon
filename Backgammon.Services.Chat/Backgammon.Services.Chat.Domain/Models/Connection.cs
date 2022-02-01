using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Domain.Models
{
    public class Connection
    {
        public string Id { get; set; }
        public Guid ChaterId { get; set; }
        public bool IsClosed { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
    }
}
