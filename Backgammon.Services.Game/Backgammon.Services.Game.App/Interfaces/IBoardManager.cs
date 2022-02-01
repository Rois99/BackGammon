using Backgammon.Services.Game.Domain.Interfaces;
using Backgammon.Services.Game.Domain.Models;
using System.Collections.Generic;

namespace Backgammon.Services.Game.App.Interfaces
{
    public interface IBoardManager
    {
        public ICubeService CubeService { get; }
        public Dictionary<string, GamePlay> OnlineGames { get; set; }
        public Dictionary<string, GameRequest> GameRequests { get; set; }


        public string AcceptGame(GameRequest gameRequest, string firstConnection, string secondConnection); //RemoveTheGameREquestAndCreateANewGame
        public void FinishGame(string GameId);
        public bool IsConnectionExistsInGame(string gameId, string playersConnection);
        public TwoNums RollCubes();
        public StartGameModel GettingStartModel(string firstPlayer, string secondPlayer);
    }
}
