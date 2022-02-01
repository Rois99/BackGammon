using Backgammon.Services.Game.Domain.Interfaces;
using Backgammon.Services.Game.Domain.Models;
using System.Collections.Generic;

namespace Backgammon.Services.Game.App.Interfaces
{
    public interface IPlayerService
    {
        public Dictionary<string, Player> Players { get; }
        public IRepository _repository { get; }

        public bool ConnectUser(string PlayerID, string UserName, string ConnectionString);
        public bool DiscconectPlayer(string playerId);
        public bool IsUserConnected(string UserID);
        public IReadOnlyList<string> GetAllConnectedUserIds(string userID);
        public bool IsAvalable(string PlayerID);
        public bool IfInGameGetGameID(string playerId, out string GameId);
        public void StartGame(GamePlay game);
        public void FinishGame(GameResult gameResult);
        //GetPlayerStats
        public PlayerStats GetPlayerStats(string playerID) => new PlayerStats();
        //inside methods:
        //public int GetPlayersWinPrecentage(string playerID);
        //public int GetPlayersTotalPlayes(string playerID);


    }
}
