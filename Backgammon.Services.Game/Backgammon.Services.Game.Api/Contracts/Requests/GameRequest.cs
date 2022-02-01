using System;

namespace Backgammon.Services.Game.Api.Contracts.Requests
{
    public class GameRequest
    {
        public string RequestId { get; set; }
        public string SenderId { get; set; }
    }
}
