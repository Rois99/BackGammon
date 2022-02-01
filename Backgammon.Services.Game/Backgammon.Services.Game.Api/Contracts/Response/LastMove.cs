using Backgammon.Services.Game.Api.Contracts.Requests;
using Backgammon.Services.Game.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Api.Contracts.Response
{
    public class LastMove
    {
        public Move OpponentMove { get; set; }
        public TwoNums YourDices { get; set; }
    }
}
