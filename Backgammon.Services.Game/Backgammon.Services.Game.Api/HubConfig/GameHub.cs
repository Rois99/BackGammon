using Backgammon.Services.Game.Api.Contracts.Dtos;
using Backgammon.Services.Game.Api.Contracts.Requests;
using Backgammon.Services.Game.Api.Contracts.Response;
using Backgammon.Services.Game.Api.Extantions;
using Backgammon.Services.Game.App.Interfaces;
using Backgammon.Services.Game.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Api.HubConfig
{
    [Authorize]
    public class GameHub : Hub
    {
        IBoardManager _boardManager;
        IPlayerService _playerService;

        public GameHub(IBoardManager boardManager, IPlayerService playerService)
        {
            _boardManager = boardManager;
            _playerService = playerService;
        }

        public async override Task OnConnectedAsync()
        {
            string PlayerID = Context.User.getPlayerId();
            string PlayerName = Context.User.FindFirst(c => c.Type == "name").Value;
            if (_playerService.ConnectUser(PlayerID, PlayerName, Context.ConnectionId))
            {
                var player = _playerService.Players[PlayerID];
                await Clients.Users(_playerService.GetAllConnectedUserIds(Context.User.getPlayerId()))
                    .SendAsync("NewPlayerJoined", new PlayerDto
                    {
                        Id = player.ID,
                        Name = player.UserName,
                        InGame = player.InGame
                    });
            }
            await Clients.Caller.SendAsync("Connected", _playerService.Players.Values
                    .Select(p => new PlayerDto
                    {
                        Id = p.ID,
                        InGame = p.InGame,
                        Name = p.UserName
                    }).ToList());

        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            string playerID = Context.UserIdentifier;
            if (_playerService.IfInGameGetGameID(playerID, out string gameID))//if he was in game
            {
                if (_boardManager.IsConnectionExistsInGame(gameID, Context.ConnectionId))
                {
                    string otherConnectionPlayerID = _boardManager.OnlineGames[gameID].GetOthersPlayerConnection(playerID);
                    RemoveFromGame(gameID, playerID);
                    await Clients.Client(otherConnectionPlayerID).SendAsync("GameError", GameErrors.OponentDisconnected);
                }
            }

            if (_playerService.DiscconectPlayer(playerID))//checks if its his last connection -- It is also disconnects him
                await Clients.All.SendAsync("PlayerDisconnected", playerID);
        }

        public async Task SendGameRequest(string reciverID)
        {
            if (!_playerService.IsAvalable(Context.UserIdentifier))
            {
                await Clients.Caller.SendAsync("GameError", GameErrors.YouAreInGame);
                return;
            }
            if (!_playerService.IsAvalable(reciverID))//checking if not availble
            {
                await Clients.Caller.SendAsync("GameError", GameErrors.OpponentInGame);
                return;
            }
            string senderID = Context.User.getPlayerId();
            var gameRequest = new Domain.Models.GameRequest() { RequestId = Guid.NewGuid().ToString(), SenderID = senderID, RecieverID = reciverID, SenderConnection = Context.ConnectionId };
            _boardManager.GameRequests.Add(gameRequest.RequestId, gameRequest);
            await Clients.Users(reciverID).SendAsync("GameRequest", new Contracts.Requests.GameRequest { SenderId = senderID, RequestId = gameRequest.RequestId });
        }

        public async Task GameRequestApproved(RequestResponse response)//Try To Start the game and to send the Players The StartGameModel 
        {
            string receiverId = Context.User.getPlayerId();
            Domain.Models.GameRequest gameRequest = _boardManager.GameRequests[response.RequestId];
            string senderConnectionString = gameRequest.SenderConnection;
            string reciverConnectionString = Context.ConnectionId;
            if (!response.IsAccepted)
            {
                await Clients.Client(senderConnectionString).SendAsync("RequestDenied", receiverId);
                return;
            }
            if (_playerService.IsAvalable(gameRequest.SenderID)) // if true starts a game
            {
                string gameID = _boardManager.AcceptGame(gameRequest, senderConnectionString, reciverConnectionString);//Removes the game request and create a new game
                GamePlay newGame = _boardManager.OnlineGames[gameID];
                _playerService.StartGame(newGame);
                StartGameModel startGameModel = _boardManager.GettingStartModel(newGame.FirstPlayerID, newGame.SecondPlayerID);
                startGameModel.GameId = gameID;
                newGame.InitGamePlay(startGameModel);//InitThePlayersTurnAndNumOfMovesLeft
                await Clients.Client(senderConnectionString).SendAsync("StartGame", startGameModel);
                await Clients.Client(Context.ConnectionId).SendAsync("StartGame", startGameModel);
                return;
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("GameError", GameErrors.SenderUnavilable);
                return;
            }
        }

        public async Task SendMove(Move move)
        {
            Colors playersColor;
            var gameID = move.GameId;
            GamePlay game = _boardManager.OnlineGames[gameID];
            if (Context.User.getPlayerId() != game.CurrentPlayersTurn)//אם הוא לא שלח בתורו
                return;

            if (Context.User.getPlayerId() == game.FirstPlayerID)
                playersColor = Colors.Player1;
            else
                playersColor = Colors.Player2;

            var playersMove = new PlayersMove
            {
                CellNumber = move.StackNumber,
                NumOfSteps = move.NumOfSteps,
                PlayersColor = playersColor
            };

            string CurrentPlayerConnection = Context.ConnectionId;
            string OtherPlayerConnection = game.GetOthersPlayerConnection(Context.UserIdentifier);

            int CheckAvalblility = game.CheckAvailbilty(playersMove);//MoveStatus

            if (CheckAvalblility == -1)//checks All The Options
            {
                await Clients.Client(CurrentPlayerConnection).SendAsync("GameError", GameErrors.IlligelMove);//Sends the id of the cheating player
                return;
            }
            if (CheckAvalblility == -2)//checks All The Options
            {
                await Clients.Client(CurrentPlayerConnection).SendAsync("GameError", GameErrors.PlayerUseCheats);//Sends the id of the cheating player
                return;
            }

            var HasWon = game.MoveTrueIfWins(playersMove, CheckAvalblility);//makes all the moves and The Board changes
            if (HasWon)
            {
                string LossersID;

                if (Context.User.getPlayerId() == game.FirstPlayerID)
                    LossersID = game.SecondPlayerID;
                else
                    LossersID = game.FirstPlayerID;
                await FinishGame(Context.User.getPlayerId(), LossersID, game.GameId);//storing the game information and ends the game
                return;
            }
            if (!game.CurrentPlayes.HasMoreNumbers())
            {
                await SwitchPlayers(game, CurrentPlayerConnection, OtherPlayerConnection, playersColor, playersMove, false);
            }
            else //If he didnt used all his Cubes
            {
                if (game.IsThereMoreMovements(playersColor))
                {
                    await Clients.Client(OtherPlayerConnection).SendAsync("OpponentMove", new Move { GameId = gameID, NumOfSteps = -move.NumOfSteps, StackNumber = 25 - move.StackNumber });
                }
                else
                {
                    await SwitchPlayers(game, CurrentPlayerConnection, OtherPlayerConnection, playersColor, playersMove, true);
                }
            }

        }

        private async Task SwitchPlayers(GamePlay game, string CurrentPlayerConnection, string OtherPlayerConnection, Colors playersColor, PlayersMove playersMove, bool isSkipped)
        {
            var gameID = game.GameId;
            var newNums = _boardManager.RollCubes();

            game.switchPlayersTurnAndRollCubes(newNums);

            await Clients.Client(CurrentPlayerConnection).SendAsync("TurnIsOver", new TurnOver { NewNums = newNums, Skipped = isSkipped });
            if (playersColor == Colors.Player1)
                await Clients.Client(OtherPlayerConnection).SendAsync("LastMoveOfOpponent", new LastMove() { OpponentMove = new Move { GameId = gameID, NumOfSteps = -playersMove.NumOfSteps, StackNumber = 25 - playersMove.CellNumber }, YourDices = newNums });
            else
                await Clients.Client(OtherPlayerConnection).SendAsync("LastMoveOfOpponent", new LastMove() { OpponentMove = new Move { GameId = gameID, NumOfSteps = playersMove.NumOfSteps, StackNumber = playersMove.CellNumber }, YourDices = newNums });

            var playerNow = playersColor == Colors.Player1 ? Colors.Player2 : Colors.Player1;

            if (!game.IsThereMoreMovements(playerNow))
            {
                await Task.Delay(3000);
                await SwitchPlayers(game, OtherPlayerConnection, CurrentPlayerConnection, playerNow, new PlayersMove { CellNumber = 0, NumOfSteps = 0, PlayersColor = playersColor }, true);
            }
        }

        public async Task FinishGame(string WinnerID, string LosserID, string GameID)
        {
            GamePlay game = _boardManager.OnlineGames[GameID];
            GameResult gameResult = new GameResult() { WinnerId = WinnerID, LosserId = LosserID, GameId = GameID };

            _playerService.FinishGame(gameResult);//מעדכנת את 
            _boardManager.FinishGame(GameID);
            await Clients.Client(game.FirstPlayerConnection).SendAsync("FinishGame", gameResult);
            await Clients.Client(game.SecondPlayerConnection).SendAsync("FinishGame", gameResult);

        }

        public async Task GetPlayerStats(string PlayersID)
        {
            PlayerStats playerStats = _playerService.GetPlayerStats(PlayersID);
            await Clients.Caller.SendAsync("getPlayerStats", playerStats);
            return;
        }

        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong");
        }

        private void RemoveFromGame(string gameId,string playerId)
        {
            var otherPlayerId = _boardManager.OnlineGames[gameId].GetOtherPlayersID(playerId);
            _boardManager.OnlineGames.Remove(gameId);

            if(_playerService.Players.TryGetValue(playerId,out var player))
            {
                player.GameId = "e";
            }

            if (_playerService.Players.TryGetValue(otherPlayerId, out var player2))
            {
                player2.GameId = "e";
            }
        }
    }
}
