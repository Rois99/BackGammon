using Backgammon.Services.Chat.Domain.Interfaces;
using Backgammon.Services.Chat.Domain.Models;
using Backgammon.Services.Chat.Infra.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Infra.Data.Repositorys
{
    public class ChatterRepo : IChatterRepo
    {
        private readonly DataContext context;

        public ChatterRepo(DataContext context)
        {
            this.context = context;
        }

        public async Task<Chatter> AddChatter(Guid chatterId,string name)
        {
            var chatter = new ChatterDto()
            {
                Id = chatterId,
                Name = name,
                IsConnected = false
            };
            await context.Chatters.AddAsync(chatter);
            await context.SaveChangesAsync();
            return new Chatter
            {
                Id = chatter.Id,
                Name = chatter.Name,
                IsConnected = chatter.IsConnected
            };
        }

        public async Task<Chatter> FindChatter(Guid chatterId)
        {
            var chatter = await context.Chatters.FindAsync(chatterId);
            if (chatter == null)
                return null;
            return new Chatter { 
                Id = chatter.Id,
                Name = chatter.Name,
                IsConnected = chatter.IsConnected
            };
        }

        public Task<IEnumerable<Chatter>> GetChatters()
        {
            return Task.FromResult<IEnumerable<Chatter>>(context.Chatters.Select(c =>new Chatter{
                    Id = c.Id,
                    Name = c.Name,
                    IsConnected = c.IsConnected,
                    LastSeen = c.Connections.Max(c => c.EndedAt)
               }).ToList());
        }

        public async Task SetConnected(Guid cahatterId)
        {
            var chatter = await context.Chatters.FindAsync(cahatterId);
            if (chatter == null)
                throw new ArgumentException($"No chatter with id:: {cahatterId}");

            chatter.IsConnected = true;
            await context.SaveChangesAsync();
        }

        public async Task SetDisconnected(Guid cahatterId)
        {
            var chatter = await context.Chatters.FindAsync(cahatterId);
            if (chatter == null)
                throw new ArgumentException($"No chatter with id:: {cahatterId}");

            chatter.IsConnected = false;
            await context.SaveChangesAsync();
        }
    }
}
