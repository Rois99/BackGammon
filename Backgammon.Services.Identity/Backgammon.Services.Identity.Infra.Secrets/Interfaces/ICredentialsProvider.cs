using Microsoft.IdentityModel.Tokens;

namespace Backgammon.Services.Identity.Infra.Secrets.Interfaces
{
    public interface ICredentialsProvider
    {
        SigningCredentials GetSigningCredentials();
        SecurityKey GetSingnigKey();

    }
}
