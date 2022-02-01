using System.Collections.Generic;

namespace Backgammon.Services.Identity.Application.Results
{
    public class FailedResult : Result
    {
        public IEnumerable<string> Errors { get; set; }

        public FailedResult() : base(false)
        {

        }
    }
}
