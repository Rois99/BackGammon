using System.Collections.Generic;

namespace Backgammon.Services.Game.Api.Contracts.Response
{
    public class ErrorResponse : Response
    {
        public List<string> Errors { get; set; }

        public ErrorResponse(List<string> erros)
        {
            IsSuccses = false;
            Errors = erros;
        }

        public ErrorResponse()
        {
            IsSuccses = false;
        }
    }
}
