using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Backgammon.Services.Identity.Infra.Data.Dtos
{
    public class UserDto : IdentityUser<Guid>
    {
        public virtual IEnumerable<RefreshTokenDto> RefreshTokens { get; set; }
    }
}
