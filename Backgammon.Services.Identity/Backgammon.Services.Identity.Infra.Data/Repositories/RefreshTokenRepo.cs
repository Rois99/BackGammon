using System;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Domain.Interfaces;
using Backgammon.Services.Identity.Infra.Data.Contexts;
using Backgammon.Services.Identity.Infra.Data.Dtos;

namespace Backgammon.Services.Identity.Infra.Data.Repositories
{
    public class RefreshTokenRepo : IRefreshTokenProvider
    {
        private readonly DataContext dataContext;
        private readonly TimeSpan expiringTime = new TimeSpan(182,0,0,0);

        public RefreshTokenRepo(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IRefreshToken CreateToken(Guid userId, Guid jwtId)
        {
            var user = dataContext.Users.Find(userId);
            if (user == null)
                throw new ArgumentException("User doesn't exist.");

            var refreshTokenDto = new RefreshTokenDto()
            {
                UserId = userId,
                JwtId = jwtId,
                IssuedAt = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.Add(expiringTime),
                IsUsed = false,
                IsInvalidated = false
            };
            dataContext.RefreshTokens.Add(refreshTokenDto);
            dataContext.SaveChanges();

            return refreshTokenDto;
        }

        public async Task<IRefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            if (!Guid.TryParse(refreshToken, out var id))
                return null;

            var token = await dataContext.RefreshTokens.FindAsync(id);
            return token;
        }

        public async Task InvalidetRefreshTokenAsync(IRefreshToken refreshToken)
        {
            ((RefreshTokenDto)refreshToken).IsInvalidated = true;
            await dataContext.SaveChangesAsync();
        }

        public async Task UseRefreshTokenAsync(IRefreshToken refreshToken)
        {
            ((RefreshTokenDto)refreshToken).IsUsed = true;
            await dataContext.SaveChangesAsync();
        }

        public string WriteToken(IRefreshToken token) => token.Token.ToString();
    }
}
