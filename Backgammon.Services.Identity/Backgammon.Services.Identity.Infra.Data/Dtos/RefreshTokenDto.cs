using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backgammon.Services.Identity.Domain.Interfaces;

namespace Backgammon.Services.Identity.Infra.Data.Dtos
{
    public class RefreshTokenDto : IRefreshToken
    {
        [Key]
        public Guid Token { get; set; }
        public Guid JwtId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsUsed { get; set; }
        public bool IsInvalidated { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserDto User { get; set; }
    }
}
