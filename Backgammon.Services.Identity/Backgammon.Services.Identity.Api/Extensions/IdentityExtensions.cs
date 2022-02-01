using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backgammon.Services.Identity.Api.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
        => user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? user.FindFirstValue(ClaimTypes.NameIdentifier);       
    }
}
