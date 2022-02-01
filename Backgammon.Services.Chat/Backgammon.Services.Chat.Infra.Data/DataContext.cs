using Backgammon.Services.Chat.Infra.Data.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Backgammon.Services.Chat.Infra.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {

        }

        public DbSet<ChatterDto> Chatters { get; set; }
        public DbSet<ConnectionDto> Connections { get; set; }
        public DbSet<MessageDto> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);       
        }
    }
}
