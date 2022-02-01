using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Api.Contracts.Requests
{
    public class ConfirmRequest
    {
        public Guid MessageId { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
