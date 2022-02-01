using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Models
{
    public class PlayersMove
    {
        public Colors PlayersColor { get; set; }
        public int CellNumber { get; set; }
        public int NumOfSteps { get; set; }
    }
}
