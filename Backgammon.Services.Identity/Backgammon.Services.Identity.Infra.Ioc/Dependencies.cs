using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Backgammon.Services.Identity.Domain.Interfaces;
using Backgammon.Services.Identity.Infra.Data.Contexts;
using Backgammon.Services.Identity.Infra.Data.Dtos;
using Backgammon.Services.Identity.Infra.Data.Managers;
using Backgammon.Services.Identity.Infra.Data.Repositories;
using Backgammon.Services.Identity.Infra.Secrets.Interfaces;
using Backgammon.Services.Identity.Infra.Secrets.Providers;

namespace Backgammon.Services.Identity.Infra.Ioc
{
    public static class Dependencies
    {
        // Infrastructure services

        public static void AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DataContext>(options =>
            {
                var connecttionString = configuration.GetConnectionString("local");
                options.UseSqlServer(connecttionString)
                .UseLazyLoadingProxies();
            });

            // Configure Identity
            services
                .AddIdentityCore<UserDto>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = false;
                })
                .AddUserManager<UserManager>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddScoped<IUserManager, UserManager>();
        }

        public static void AddRefreshTokens(this IServiceCollection services)
        {
            services.AddScoped<IRefreshTokenProvider, RefreshTokenRepo>();
        }

        public static void AddSecrets(this IServiceCollection services)
        {
            services.AddSingleton<ICredentialsProvider,CredentialsProvider>();
            services.AddSingleton<IJsonWebTokenProvider, JwtProvider>();
        }

        public static void AddSecrets(this IServiceCollection services,Action<ICredentialsProvider> signigAction)
        {
            var provider = new CredentialsProvider();
            services.AddSingleton<ICredentialsProvider>(provider);
            services.AddSingleton<IJsonWebTokenProvider, JwtProvider>();
            signigAction?.Invoke(provider);
        }
    }
}
