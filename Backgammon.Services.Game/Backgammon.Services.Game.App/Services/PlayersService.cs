using Backgammon.Services.Game.App.Interfaces;
using Backgammon.Services.Game.Domain.Interfaces;
using Backgammon.Services.Game.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Backgammon.Services.Game.App.Services
{
    public class PlayersService : IPlayerService
    {
        private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

        public Dictionary<string, Player> Players => _players;

        public IRepository _repository { get; }


        public PlayersService(IRepository repository)
        {
            _repository = repository;
        }

        public Player GetPlayerFromRepo(string Id, string ConnectionId) => _repository.GetPlayerFromRepo(Id, ConnectionId); //getsPlayerFromRepositor

        public bool IsUserConnected(string UserID)
        {
            if (Players.TryGetValue(UserID, out Player player))
                return true;
            else
                return false;
        }

        public IReadOnlyList<string> GetAllConnectedUserIds(string except) //returns all the connections of all the players besides the ChosenOne
        {
            var playerIds = Players
                .Where(p => p.Key != except)
                .Select(p => p.Key).ToList();
            return playerIds;
        }

        public bool ConnectUser(string PlayerID, string UserName, string ConnectionString) //returns false if the player alredy online
        {
            if (IsUserConnected(PlayerID))
            {
                Players[PlayerID].NumberOfConnections++;
                return false;
            }
            if (!_repository.IsExist(PlayerID))
            {
                _repository.AddNewPlayer(PlayerID, UserName);
            }
            var connectedPlayer = _repository.GetPlayerFromRepo(PlayerID, ConnectionString);
            Players.Add(PlayerID, connectedPlayer);

            return true;
        }

        public bool IfInGameGetGameID(string playerId, out string GameId)
        {
            if (Players.TryGetValue(playerId, out Player player))
            {
                if (player.InGame)
                {
                    GameId = player.GameId;
                    return true;
                }
            }
            GameId = "e";
            return false;
        }

        public bool DiscconectPlayer(string playerId)
        {
            if (Players[playerId].NumberOfConnections == 1)
            {
                Players.Remove(playerId);
                return true;
            }
            Players[playerId].NumberOfConnections--;
            return false;
        }

        public void StartGame(GamePlay game)
        {
            Players[game.FirstPlayerID].GameId = game.GameId;
            Players[game.SecondPlayerID].GameId = game.GameId;
        }

        public bool IsAvalable(string PlayerID)
        {
            if (Players.TryGetValue(PlayerID, out Player p))
                if (!p.InGame)
                    return true;
            return false;
        }

        public void FinishGame(GameResult gameResult)
        {
            Players.TryGetValue(gameResult.WinnerId, out Player Winner);
            Players.TryGetValue(gameResult.LosserId, out Player Losser);
            Winner.GameId = "e";
            GameResultToHistory WinnersGame = new GameResultToHistory() { PlayerDtoID = gameResult.WinnerId, ID = gameResult.GameId + "W", HasWon = true, ComponentsID = gameResult.LosserId };
            Winner.GameHistory.Add(WinnersGame);
            Losser.GameId = "e";
            GameResultToHistory LosserGame = new GameResultToHistory() { PlayerDtoID = gameResult.LosserId, ID = gameResult.GameId + "L", HasWon = false, ComponentsID = gameResult.WinnerId };
            Losser.GameHistory.Add(LosserGame);
            _repository.UpdatePlayersResutlsAfterGame(WinnersGame, LosserGame);

        }

        private int GetPlayersWinPrecentage(string playerID)
        {
            Players.TryGetValue(playerID, out Player player);
            int wins = player.GameHistory.Where(g => g.HasWon == true).Count();
            int losses = player.GameHistory.Where(g => g.HasWon == false).Count();
            if (wins == 0)
                return 0;
            return (wins / (wins + losses)) * 100;
        }

        private int GetPlayersTotalPlayes(string playerID)
        {
            Players.TryGetValue(playerID, out Player player);
            return player.GameHistory.Count();
        }

        public PlayerStats GetPlayerStats(string playerID)
        {
            if (!Players.TryGetValue(playerID, out _))
                return null;
            return new PlayerStats()
            {
                GamePlayed = GetPlayersTotalPlayes(playerID),
                WinPrecentage = GetPlayersWinPrecentage(playerID)
            };

        }
    }
}
