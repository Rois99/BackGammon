using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Api.Contracts.Response
{
    public class RequestResponse
    {
        public string RequestId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
