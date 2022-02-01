using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Application.Interfaces;
using Backgammon.Services.Identity.Application.Results;
using Backgammon.Services.Identity.Domain.Interfaces;

namespace Backgammon.Services.Identity.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserManager manager;
        private readonly IJsonWebTokenProvider accessTokenProvider;
        private readonly IRefreshTokenProvider refreshTokenProvider;

        public AuthService(IUserManager manager,IJsonWebTokenProvider accessTokenProvider,IRefreshTokenProvider refreshTokenProvider)
        {
            this.manager = manager;
            this.accessTokenProvider = accessTokenProvider;
            this.refreshTokenProvider = refreshTokenProvider;
        }

        public async Task<Result> LoginAsync(string username, string password)
        {
            if (!await manager.IsExistByUserNameAsync(username))
                return new FailedResult()
                {
                    Errors = new[] { "User doesn't exist." }
                };

            if (!await manager.CheckPassword(username, password))
                return new FailedResult()
                {
                    Errors = new[] { "Incorrect password." }
                };

            return await GenerateTokensForUser(username);
        }

        public async Task<Result> LogoutAsync(string refreshToken)
        {
            var token = await refreshTokenProvider.GetRefreshTokenAsync(refreshToken);
            if (token == null)
                return new FailedResult() { Errors = new[] { "Invalid refresh token." } };

            await refreshTokenProvider.InvalidetRefreshTokenAsync(token);

            return new Result(true);
        }

        public async Task<Result> RefreshAsync(string accessToken, string refreshToken)
        {
            var jwt = accessTokenProvider.ReadJwtToken(accessToken);

            if (jwt == null)
                return new FailedResult()
                {
                    Errors = new[] { "Invalid token." }
                };

            if (jwt.ValidTo > DateTime.UtcNow)
                return new FailedResult
                {
                    Errors = new[] { "Token hasn't expired." }
                };

            var refreshTokne = await refreshTokenProvider.GetRefreshTokenAsync(refreshToken);

            if (refreshTokne == null)
                return new FailedResult
                {
                    Errors = new[] { "Refresh token does not exist." }
                };

            if (DateTime.UtcNow > refreshTokne.ValidTo)
                return new FailedResult
                {
                    Errors = new[] { "Refresh token has expired." }
                };

            if (refreshTokne.IsInvalidated)
                return new FailedResult
                {
                    Errors = new[] { "Refresh token has been invalidated." }
                };

            if (refreshTokne.IsUsed)
                return new FailedResult
                {
                    Errors = new[] { "Refresh token has been used." }
                };

            if(refreshTokne.JwtId != jwt.Id)
                return new FailedResult
                {
                    Errors = new[] { "Refresh token does not match the JWT." }
                };

            await refreshTokenProvider.UseRefreshTokenAsync(refreshTokne);

            return await GenerateTokensForUser(jwt.UserName);
        }

        public async Task<Result> ValidateUser(Guid userId, string password)
        {

            if (!await manager.IsExistByIdAsync(userId))
                return new FailedResult()
                {
                    Errors = new[] { "User doesn't exist." }
                };

            if (!await manager.CheckPassword(userId, password))
                return new FailedResult()
                {
                    Errors = new[] { "Incorrect password." }
                };

            return new Result(true);
        }

        private async Task<AuthResult> GenerateTokensForUser(string username)
        {
            var claims = await manager.GetUserClaimsAsync(username);
            var guid = Guid.NewGuid().ToString();

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, guid));

            var accessToken = accessTokenProvider.CreateToken(claims);

            var refreshToken = refreshTokenProvider.CreateToken(accessToken.UserId, accessToken.Id);

            return new AuthResult()
            {
                UserId = accessToken.UserId,
                AccessToken = accessTokenProvider.WriteToken(accessToken),
                RefreshToken = refreshTokenProvider.WriteToken(refreshToken)
            };
        }
    }
}
