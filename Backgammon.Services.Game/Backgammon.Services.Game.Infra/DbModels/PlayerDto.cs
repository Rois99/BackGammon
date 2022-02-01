using Backgammon.Services.Game.Domain.Models;
using System.Collections.Generic;

namespace Backgammon.Services.Game.Infra.DbModels
{
    public class PlayerDto
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public ICollection<GameResultToHistory> GamesHistory { get; set; }

        public PlayerDto() { }

        public PlayerDto(string Id, string userName)
        {
            ID = Id;
            UserName = userName;
            GamesHistory = new List<GameResultToHistory>();
        }
    }
}
