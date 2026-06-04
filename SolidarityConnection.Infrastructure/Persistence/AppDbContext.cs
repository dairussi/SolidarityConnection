using Microsoft.EntityFrameworkCore;
using SolidarityConnection.Domain.Models;

namespace SolidarityConnection.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).HasMaxLength(150).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
                entity.Property(u => u.Cpf).HasMaxLength(14);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(50).IsRequired();
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });
        }
    }
}
