using Backgammon.Services.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Application.Interfaces
{
    public interface IChatService
    {
        Task<Chatter> GetOrAddChatterAsync(Guid chaterId,string username);
        Task<DateTime> GetLastSeen(Guid chatterId);
        Task<Chatter> GetChatterAsync(Guid chatterId);
        Task<bool> ConnectChatterAsync(Guid chater,string connectionId);
        Task<bool> DisconnectChatterAsync(Guid chater,string connectionId);
        Task<IEnumerable<Chatter>> GetChattersAsync(Guid exlodedId);
        void CloseAllConnections();
    }
}
