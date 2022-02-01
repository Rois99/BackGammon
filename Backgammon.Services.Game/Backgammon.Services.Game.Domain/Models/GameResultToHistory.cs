using System.ComponentModel.DataAnnotations.Schema;

namespace Backgammon.Services.Game.Domain.Models
{
    public class GameResultToHistory
    {
        public string ID { get; set; }
        public string PlayerDtoID { get; set; }
        public bool HasWon { get; set; }
        public string ComponentsID { get; set; }
    }
}
