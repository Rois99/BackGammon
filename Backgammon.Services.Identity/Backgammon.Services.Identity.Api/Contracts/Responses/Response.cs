using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Identity.Contracts.Responses
{
    public class Response
    {
        public bool IsSuccess { get; set; }

        public Response(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}
