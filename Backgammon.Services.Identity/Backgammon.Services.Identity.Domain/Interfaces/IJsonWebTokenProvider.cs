using System.Collections.Generic;
using System.Security.Claims;

namespace Backgammon.Services.Identity.Domain.Interfaces
{
    public interface IJsonWebTokenProvider
    {
        IJsonWebToken ReadJwtToken(string token);
        IJsonWebToken CreateToken(IEnumerable<Claim> claims);
        string WriteToken(IJsonWebToken token);
    }
}
