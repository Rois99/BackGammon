using System;

namespace Backgammon.Services.Identity.Domain.Interfaces
{
    public interface IRefreshToken
    {
        public Guid Token { get; }
        public Guid JwtId { get;  }
        public DateTime IssuedAt { get; }
        public DateTime ValidTo { get; }
        public bool IsUsed { get; }
        public bool IsInvalidated { get;  }
        public Guid UserId { get; }
    }
}
