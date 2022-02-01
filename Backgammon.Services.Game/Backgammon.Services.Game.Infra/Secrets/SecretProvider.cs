using Backgammon.Services.Game.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Infra.Secrets
{
    public class SecretProvider : ISecretProvider
    {
        private const string secret = "this is a secret";
        private readonly byte[] key = Encoding.ASCII.GetBytes(secret);

        public SecurityKey GetSingnigKey() => new SymmetricSecurityKey(key);
    }
}
