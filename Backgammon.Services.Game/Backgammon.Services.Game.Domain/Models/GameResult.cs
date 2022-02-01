using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Models
{
    public class GameResult
    {
        public string GameId { get; set; }
        public string WinnerId { get; set; }
        public string LosserId { get; set; }
    }
}
