using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Models
{
    public class StartGameModel
    {
        public string GameId { get; set; }
        public string PlayerOne { get; set; }
        public string PlayerTwo { get; set; }
        public TwoNums WhosFirstCubes { get; set; }
        public TwoNums PlayingCubes { get; set; }
    }
}
