using Microsoft.IdentityModel.Tokens;
using System;

namespace Backgammon.Services.Identity.Domain.Interfaces
{
    public interface IJsonWebToken
    {
        public SecurityToken Token { get; }
        public Guid UserId { get; }
        public Guid Id { get; }
        public string UserName { get; }
        public DateTime ValidTo { get; }
        public DateTime IssuedAt { get;  }
    }
}
