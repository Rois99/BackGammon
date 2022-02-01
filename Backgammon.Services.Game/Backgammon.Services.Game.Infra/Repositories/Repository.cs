using Backgammon.Services.Game.Domain.Interfaces;
using Backgammon.Services.Game.Domain.Models;
using Backgammon.Services.Game.Infra.DbModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Backgammon.Services.Game.Infra.Repositories
{
    public class Repository : IRepository
    {
        GameDataContext _dataContext;

        public Repository(GameDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool IsExist(string playerId) // returns false if it is a new player
        {
            var player = _dataContext.Players.Find(playerId);
            if (player is default(PlayerDto))
                return false;
            else
                return true;
        }

        public Player GetPlayerFromRepo(string Id, string connectionId) //get player from db 
        {
            PlayerDto pDto = _dataContext.Players.Find(Id);
            _dataContext.Entry(pDto).Collection(p => p.GamesHistory).Load();
            Player player = new Player(pDto.ID, pDto.UserName, connectionId, pDto.GamesHistory.ToList());
            return player;
        }

        public void AddNewPlayer(string Id, string userName)
        {
            _dataContext.Players.Add(new PlayerDto(Id, userName));
            _dataContext.SaveChanges();
        }

        public void UpdatePlayersResutlsAfterGame(GameResultToHistory winnerResult, GameResultToHistory looserResult) //מעדכן את שתי חברי המשחק בתוצאות 
        {
            PlayerDto winnerPlayer = _dataContext.Players.Find(winnerResult.PlayerDtoID);
            _dataContext.Entry(winnerPlayer).Collection(p => p.GamesHistory).Load();
            winnerPlayer.GamesHistory.Add(winnerResult);

            PlayerDto losserPlayer = _dataContext.Players.Find(looserResult.PlayerDtoID);
            _dataContext.Entry(losserPlayer).Collection(p => p.GamesHistory).Load();
            losserPlayer.GamesHistory.Add(looserResult);

            _dataContext.SaveChanges();
        }

        //public GameResultsDto ConvertGameResultToDto(GameResultToHistory game)=> new GameResultsDto() { Player = GetPlayerByID(game.PlayerID), ID = game.ID, HasWon = game.HasWon, ComponentsID = game.ComponentsID };
        //public GameResultToHistory ConvertGameResultFromDto(GameResultsDto game) => new GameResultToHistory() { PlayerID = GetPlayerByID(game.Player.ID).ID, ID = game.ID, HasWon = game.HasWon, ComponentsID = game.ComponentsID };

    }
}
