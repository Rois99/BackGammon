using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backgammon.Services.Identity.Domain.Interfaces;
using Backgammon.Services.Identity.Infra.Secrets.Interfaces;
using Backgammon.Services.Identity.Infra.Secrets.Tokens;

namespace Backgammon.Services.Identity.Infra.Secrets.Providers
{
    public class JwtProvider : JwtSecurityTokenHandler, IJsonWebTokenProvider
    {
        private readonly TimeSpan expiringTime = new(0, 0, 20);
        private readonly ICredentialsProvider credentialsProvider;

        public JwtProvider(ICredentialsProvider credentialsProvider)
        {
            this.credentialsProvider = credentialsProvider;
        }

        public IJsonWebToken CreateToken(IEnumerable<Claim> claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Issuer = settings.Issuer,
                //Audience = audience,
                Expires = DateTime.UtcNow.Add(expiringTime),
                SigningCredentials = credentialsProvider.GetSigningCredentials()
            };
            return new JsonWebToken(CreateJwtSecurityToken(tokenDescriptor));
        }

        IJsonWebToken IJsonWebTokenProvider.ReadJwtToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException();
            var tvp = new TokenValidationParameters()
            {
                //ValidateIssuer = true,
                //ValidateAudience = true,
                RequireExpirationTime = true,
                //ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                //ClockSkew = TimeSpan.Zero,
                //ValidIssuer = authSettings.Issuer,
                //ValidAudiences = authSettings.Audiences,
                IssuerSigningKey = credentialsProvider.GetSingnigKey(),
            };

            try
            {
                var jwt = ValidateSignature(token, tvp);
                return new JsonWebToken(jwt);
            }
            catch(SecurityTokenException)
            {
                return null;
            }                   
        }

        public string WriteToken(IJsonWebToken token) => WriteToken(token.Token);
    }
}
