using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Models
{
    public enum Colors { NoColor, Player1, Player2 };
    public class GameCell
    {
        public int NumOfPieces { get; set; }
        public Colors Color { get; set; }

        public GameCell()
        {
            NumOfPieces = 0;
            Color = Colors.NoColor;
        }
    }
}
