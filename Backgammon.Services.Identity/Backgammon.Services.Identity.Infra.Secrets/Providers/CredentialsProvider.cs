using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backgammon.Services.Identity.Infra.Secrets.Interfaces;

namespace Backgammon.Services.Identity.Infra.Secrets.Providers
{
    public class CredentialsProvider : ICredentialsProvider
    {
        private const string secret = "this is a secret";
        private readonly byte[] key = Encoding.ASCII.GetBytes(secret);

        public SigningCredentials GetSigningCredentials() =>
            new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

        public SecurityKey GetSingnigKey() => new SymmetricSecurityKey(key);

    }
}
