using System;

namespace Backgammon.Services.Identity.Application.Results
{
    public class AuthResult : Result
    {
        public AuthResult() : base(true)
        {
        }
        public Guid UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
