using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Models
{
    public class GameRequest
    {
        public string RequestId { get; set; }
        public string SenderID { get; set; }
        public string RecieverID { get; set; }
        public string SenderConnection { get; set; }
    }
}
