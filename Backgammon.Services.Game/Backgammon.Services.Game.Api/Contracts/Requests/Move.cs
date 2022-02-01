using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Api.Contracts.Requests
{
    public class Move
    {
        public string GameId { get; set; }
        public int StackNumber { get; set; }
        public int NumOfSteps { get; set; }
    }
}
