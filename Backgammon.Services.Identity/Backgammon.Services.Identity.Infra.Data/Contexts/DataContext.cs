using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Backgammon.Services.Identity.Infra.Data.Dtos;

namespace Backgammon.Services.Identity.Infra.Data.Contexts
{
    public class DataContext : IdentityUserContext<UserDto, Guid>
    {

        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }

        public DbSet<RefreshTokenDto> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserDto>(b =>
            {
                b.ToTable("Users");
            });

            builder.Entity<RefreshTokenDto>(b =>
            {
                b.ToTable("RefreshTokens");
            });

            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.ToTable("UserLogins");
            });

            builder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.ToTable("UserTokens");
            });           
        }
    }
}
