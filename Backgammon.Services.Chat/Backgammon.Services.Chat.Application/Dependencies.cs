using Backgammon.Services.Chat.Application.Interfaces;
using Backgammon.Services.Chat.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Backgammon.Services.Chat.Application
{
    public static class Dependencies
    {
        public static void AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IMessagesService, MessagesService>();
        }
    }
}
