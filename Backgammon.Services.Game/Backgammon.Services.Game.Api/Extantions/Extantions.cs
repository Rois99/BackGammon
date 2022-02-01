using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Backgammon.Services.Game.Api.Extantions
{
    public static class Extantions
    {
        public static string getPlayerId(this ClaimsPrincipal user) =>
            user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
