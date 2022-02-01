using Backgammon.Services.Chat.Domain.Interfaces;
using Backgammon.Services.Chat.Domain.Models;
using Backgammon.Services.Chat.Infra.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Infra.Data.Repositorys
{
    public class ConnectionRepo : IConnectionRepo
    {
        private readonly DataContext context;

        public ConnectionRepo(DataContext context)
        {
            this.context = context;
        }

        public void CloseAllConnections()
        {
            foreach(var conn in this.context.Connections)
            {
                if (!conn.IsClosed)
                {
                    conn.IsClosed = true;
                    conn.EndedAt = DateTime.UtcNow;
                }          
            }
            context.SaveChanges();
        }

        public async Task CloseConnection(string connectionId, DateTime endedAt)
        {
            var connection = await context.Connections.FindAsync(connectionId);
            if (connection == null)
                throw new ArgumentException($"Connection doesn't exist with id: {connectionId}");

            connection.IsClosed = true;
            connection.EndedAt = endedAt;
            await context.SaveChangesAsync();
        }

        public async Task<Connection> CreateConnection(string connectionId, Guid chatterId, DateTime startedAt)
        {
            var connectionDto = new ConnectionDto
            {
                Id = connectionId,
                ChatterId = chatterId,
                StartedAt = startedAt,
                IsClosed = false
            };

            await context.Connections.AddAsync(connectionDto);

            return new Connection
            {
                Id = connectionDto.Id,
                ChaterId = connectionDto.ChatterId,
                IsClosed = connectionDto.IsClosed,
                StartedAt = connectionDto.StartedAt
            };
        }

        public async Task<IEnumerable<Connection>> GetChatterConnections(Guid chatterId)
        {
            return (await context.Chatters.FindAsync(chatterId))
                .Connections
                .Select(c => new Connection
                {
                    Id = c.Id,
                    ChaterId = c.ChatterId,
                    IsClosed = c.IsClosed,
                    StartedAt = c.StartedAt,
                    EndedAt = c.EndedAt
                })
                .ToList();
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            var conn = await context.Connections.FindAsync(connectionId);
            return new Connection
            {
                Id = conn.Id,
                ChaterId = conn.ChatterId,
                IsClosed = conn.IsClosed,
                EndedAt = conn.EndedAt,
                StartedAt = conn.EndedAt
            };
        }
    }
}
