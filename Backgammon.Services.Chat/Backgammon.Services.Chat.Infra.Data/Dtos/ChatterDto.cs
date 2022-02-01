using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backgammon.Services.Chat.Infra.Data.Dtos
{
    [Table("Chatters")]
    public class ChatterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public virtual IEnumerable<ConnectionDto> Connections { get; set; }
    }
}
