using Backgammon.Services.Game.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Domain.Interfaces
{
    public interface IRepository
    {
        public Player GetPlayerFromRepo(string Id, string ConnectionId);
        public void UpdatePlayersResutlsAfterGame(GameResultToHistory winnerResult, GameResultToHistory looserResult); //מעדכן את שתי חברי המשחק בתוצאות 
        public void AddNewPlayer(string Id, string userName);
        public bool IsExist(string playerId); // returns false if it is a new player


    }
}
