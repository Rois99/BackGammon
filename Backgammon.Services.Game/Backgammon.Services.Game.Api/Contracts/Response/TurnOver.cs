using Backgammon.Services.Game.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Api.Contracts.Response
{
    public class TurnOver
    {
        public TwoNums NewNums { get; set; }
        public bool Skipped { get; set; }
    }
}
