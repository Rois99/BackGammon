using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Models
{
    public class Player
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string GameId { get; set; }
        public List<string> Connections { get; set; }
        public List<GameResultToHistory> GameHistory { get; set; }
        public bool InGame { get => GameId != "e"; }
        public int NumberOfConnections { get; set; }

        public Player(string id, string userName, string connectionString, List<GameResultToHistory> games)
        {
            ID = id;
            UserName = userName;
            GameHistory = games;
            Connections = new List<string>();
            Connections.Add(connectionString);
            GameId = "e";
        }
    }
}
