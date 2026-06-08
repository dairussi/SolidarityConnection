using Microsoft.EntityFrameworkCore;
using SolidarityConnection.Domain.Campaign.Models;
using SolidarityConnection.Domain.User.Models;

namespace SolidarityConnection.Infrastructure.Persistence
{
    public class AppDbContext(
        DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Campaign> Campaigns => Set<Campaign>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
