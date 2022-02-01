using Backgammon.Services.Game.App.Interfaces;
using Backgammon.Services.Game.Domain.Interfaces;
using Backgammon.Services.Game.Domain.Models;
using System;
using System.Collections.Generic;

namespace Backgammon.Services.Game.App.Services
{
    public class BoardsManager : IBoardManager
    {
        public ICubeService CubeService { get => _cubeService; }
        public Dictionary<string, GamePlay> OnlineGames { get; set; }
        public Dictionary<string, GameRequest> GameRequests { get; set; }
        ICubeService _cubeService;

        public BoardsManager(ICubeService cubeService)
        {
            _cubeService = cubeService;
            OnlineGames = new();
            GameRequests = new();
        }
        public void CreateNewGame(string SenderID, string ReciverID, string gameID, string firstConnection, string secondConnection)
        {
            OnlineGames.Add(gameID, new GamePlay(SenderID, ReciverID, gameID, firstConnection, secondConnection));
        }

        public TwoNums RollCubes()
        {
            TwoNums twoNums = new();
            twoNums.FirstCube = _cubeService.RollCube().Result;
            twoNums.SecondCube = _cubeService.RollCube().Result;
            return twoNums;
        }

        public StartGameModel GettingStartModel(string firstPlayer, string secondPlayer)
        {
            bool found = false;
            StartGameModel startGameModel = new();
            while (!found)
            {
                startGameModel.WhosFirstCubes = RollCubes();
                if (!startGameModel.WhosFirstCubes.IsDouble())
                    found = true;
            }
            startGameModel.PlayingCubes = RollCubes();
            startGameModel.PlayerOne = firstPlayer;
            startGameModel.PlayerTwo = secondPlayer;
            return startGameModel;
        }

        public string AcceptGame(GameRequest gameRequest, string firstConnection, string secondConnection)//RemoveTheGameRequestAndCreateANewGame
        {
            string gameID = $"{gameRequest.SenderID}{gameRequest.RecieverID}{DateTime.Now.ToString("hh,mm,ss")}";
            CreateNewGame(gameRequest.SenderID, gameRequest.RecieverID, gameID, firstConnection, secondConnection);
            GameRequests.Remove(gameRequest.RecieverID);
            return gameID;
        }

        public bool IsConnectionExistsInGame(string gameId, string playersConnection)
        {
            if(OnlineGames.TryGetValue(gameId,out var game))
            {
                return game.FirstPlayerConnection == playersConnection || game.SecondPlayerConnection == playersConnection;
            }
            return false;
        }

        public void FinishGame(string GameId)
        {
            OnlineGames.Remove(GameId);
        }
    }
}
