using Backgammon.Services.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Domain.Interfaces
{
    public interface IChatterRepo
    {
        Task<Chatter> AddChatter(Guid chatterId, string name);
        Task<Chatter> FindChatter(Guid chatterId);
        Task<IEnumerable<Chatter>> GetChatters();
        Task SetConnected(Guid cahatterId);
        Task SetDisconnected(Guid cahatterId);
    }
}
