using Backgammon.Services.Game.Domain.Interfaces;
using Backgammon.Services.Game.Infra.Secrets;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Backgammon.Services.Game.Infra.Extensions
{
    public static class Extensions
    {
        public static int getRandomNumber(this string str)
        {
            int.TryParse(str[str.Length - 3] + "", out int random);
            return random;
        }

        public static void AddSecrets(this IServiceCollection services, Action<ISecretProvider> signigAction)
        {
            var provider = new SecretProvider();
            services.AddSingleton<ISecretProvider>(provider);
            signigAction?.Invoke(provider);
        }
    }
}
