using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Domain.Models;

namespace Backgammon.Services.Identity.Domain.Interfaces
{
    public interface IRefreshTokenProvider
    {
        Task<IRefreshToken> GetRefreshTokenAsync(string refreshToken);
        Task UseRefreshTokenAsync(IRefreshToken refreshToken);
        Task InvalidetRefreshTokenAsync(IRefreshToken refreshToken);
        IRefreshToken CreateToken(Guid userId, Guid jwtId);
        string WriteToken(IRefreshToken token);
    }
}
