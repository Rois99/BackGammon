using Backgammon.Services.Chat.Domain.Interfaces;
using Backgammon.Services.Chat.Infra.Data;
using Backgammon.Services.Chat.Infra.Data.Repositorys;
using Backgammon.Services.Chat.Infra.Secrets.Interfaces;
using Backgammon.Services.Chat.Infra.Secrets.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Backgammon.Services.Chat.Infra.Ioc
{
    public static class Dependencies
    {

        public static void AddSecrets(this IServiceCollection services)
        {
            services.AddSingleton<ICredentialsProvider, CredentialsProvider>();
        }

        public static void AddSecrets(this IServiceCollection services, Action<ICredentialsProvider> signigAction)
        {
            var provider = new CredentialsProvider();
            services.AddSingleton<ICredentialsProvider>(provider);
            signigAction?.Invoke(provider);
        }

        public static void AddData(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            {
                var connecttionString = configuration.GetConnectionString("local");
                options.UseSqlServer(connecttionString)
                .UseLazyLoadingProxies();
            });


            services.AddScoped<IChatterRepo, ChatterRepo>();
            services.AddScoped<IMessageRepo, MessageRepo>();
            services.AddScoped<IConnectionRepo, ConnectionRepo>();
        }
    }
}
