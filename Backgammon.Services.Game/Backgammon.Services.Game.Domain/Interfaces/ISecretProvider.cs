using Microsoft.IdentityModel.Tokens;

namespace Backgammon.Services.Game.Domain.Interfaces
{
    public interface ISecretProvider
    {
        SecurityKey GetSingnigKey();
    }
}
