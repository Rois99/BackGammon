using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Api.Contracts.Dtos
{
    public enum GameErrors
    {
        OponentDisconnected = 1,
        YouAreInGame,
        OpponentInGame,
        SenderUnavilable,
        IlligelMove,
        PlayerUseCheats
    }
}
