using Backgammon.Services.Game.Domain.Models;
using Backgammon.Services.Game.Infra.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Backgammon.Services.Game.Infra
{
    public class GameDataContext : DbContext
    {
        public GameDataContext(DbContextOptions<GameDataContext> options) : base(options)
        {
        }

        public DbSet<GameResultToHistory> GameResults { get; set; }
        public DbSet<PlayerDto> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerDto>().HasMany(p => p.GamesHistory)
                .WithOne()
                .HasForeignKey(h => h.PlayerDtoID);
        }
    }
}
