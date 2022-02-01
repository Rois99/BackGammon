using System.Collections.Generic;

namespace Backgammon.Services.Identity.Domain.Results
{
    public class ChangeNameResult
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public ChangeNameResult()
        {

        }

        public ChangeNameResult(bool isSuccess, IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }
    }
}
