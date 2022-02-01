namespace Backgammon.Services.Game.Domain.Models
{
    public class LastMoveModel
    {
        public PlayersMove LastMove { get; set; }
        public TwoNums newNums { get; set; }
    }
}
