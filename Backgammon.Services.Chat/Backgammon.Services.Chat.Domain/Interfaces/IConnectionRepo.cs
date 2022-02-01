using Backgammon.Services.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Domain.Interfaces
{
    public interface IConnectionRepo
    {
        Task<Connection> CreateConnection(string connectionId, Guid chatterId, DateTime startedAt);
        Task CloseConnection(string connectionId, DateTime endedAt);
        Task<Connection> GetConnection(string connectionId);
        Task<IEnumerable<Connection>> GetChatterConnections(Guid chatterId);
        void CloseAllConnections();

    }
}
