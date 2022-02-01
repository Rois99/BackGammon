using System.Collections.Generic;
using Backgammon.Services.Identity.Domain.Models;

namespace Backgammon.Services.Identity.Domain.Results
{
    public class CreateUserResult
    {
        public bool IsSuccess { get; set; }
        public User User { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public CreateUserResult()
        {

        }

        public CreateUserResult(bool isSuccess,User user,IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            User = user;
            Errors = errors;
        }
    }
}
