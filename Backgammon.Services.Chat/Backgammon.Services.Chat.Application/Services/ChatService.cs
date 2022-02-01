using Backgammon.Services.Chat.Application.Interfaces;
using Backgammon.Services.Chat.Domain.Interfaces;
using Backgammon.Services.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatterRepo chatterRepo;
        private readonly IConnectionRepo connectionRepo;

        public ChatService(IChatterRepo chatterRepo,IConnectionRepo connectionRepo)
        {
            this.chatterRepo = chatterRepo;
            this.connectionRepo = connectionRepo;
        }

        public void CloseAllConnections() => connectionRepo.CloseAllConnections();

        public async Task<bool> ConnectChatterAsync(Guid chatterId,string connectionId)
        {
            await connectionRepo.CreateConnection(connectionId, chatterId, DateTime.UtcNow);
            var chatter = await chatterRepo.FindChatter(chatterId);
            if (chatter.IsConnected)
                return false;
            await chatterRepo.SetConnected(chatterId);
            return true;
        }

        public async Task<bool> DisconnectChatterAsync(Guid chater, string connectionId)
        {
            await connectionRepo.CloseConnection(connectionId, DateTime.UtcNow);
            var connections = await connectionRepo.GetChatterConnections(chater);
            if (!connections.Any(c => !c.IsClosed))
            {
                await chatterRepo.SetDisconnected(chater);
                return true;
            }
            return false;
        }

        public async Task<Chatter> GetChatterAsync(Guid chatterId)
        {
            var chatter = await chatterRepo.FindChatter(chatterId);
            return chatter;
        }

        public async Task<IEnumerable<Chatter>> GetChattersAsync(Guid exlodedId)
        {
            return (await chatterRepo.GetChatters())
                .Where(c => c.Id != exlodedId)
                .ToList();
        }

        public async Task<DateTime> GetLastSeen(Guid chatterId)
        {
            var lastSeen = (await connectionRepo.GetChatterConnections(chatterId)).Max(c => c.EndedAt);
            return lastSeen;
        }

        public async Task<Chatter> GetOrAddChatterAsync(Guid chatterId, string username)
        {
            var chatter = await chatterRepo.FindChatter(chatterId);

            if (chatter != null)
                return chatter;

            chatter = await chatterRepo.AddChatter(chatterId, username);

            return chatter;
        }


    }
}
