using Microsoft.Extensions.DependencyInjection;
using Backgammon.Services.Identity.Application.Interfaces;
using Backgammon.Services.Identity.Application.Services;

namespace Backgammon.Services.Identity.Application
{
    public static class Dependencies
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();        
        }
    }
}
