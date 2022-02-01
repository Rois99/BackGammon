using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Backgammon.Services.Identity.Domain.Interfaces;

namespace Backgammon.Services.Identity.Infra.Secrets.Tokens
{
    public class JsonWebToken : IJsonWebToken
    {
        private JwtSecurityToken token;
        private Guid userId;
        public JsonWebToken(JwtSecurityToken token)
        {
            this.token = token;
        }
        public SecurityToken Token => token;
        public Guid UserId 
        { 
            get 
            {
                if (userId == Guid.Empty)
                    userId = Guid.Parse(token.Subject);
                return userId; 
            } 
        }
        public string UserName {
            get
            {
                return token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value;
            }
        }
        Guid IJsonWebToken.Id => Guid.Parse(token.Id);
        DateTime IJsonWebToken.ValidTo => token.ValidTo;
        DateTime IJsonWebToken.IssuedAt => token.IssuedAt;
    }
}
