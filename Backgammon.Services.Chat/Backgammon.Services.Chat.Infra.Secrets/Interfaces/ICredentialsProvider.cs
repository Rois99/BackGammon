using Microsoft.IdentityModel.Tokens;

namespace Backgammon.Services.Chat.Infra.Secrets.Interfaces
{
    public interface ICredentialsProvider
    {
        SigningCredentials GetSigningCredentials();
        SecurityKey GetSingnigKey();
    }
}
