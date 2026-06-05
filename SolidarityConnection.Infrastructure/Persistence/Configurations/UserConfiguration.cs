using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolidarityConnection.Domain.User.Models;

namespace SolidarityConnection.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(u => u.Id);

        entity.Property(u => u.Name)
            .HasMaxLength(150)
            .IsRequired();

        entity.Property(u => u.PasswordHash)
            .IsRequired();

        entity.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(u => u.IsActive)
            .HasDefaultValue(true);

        // OwnsOne — mapeia o ValueObject EmailAddress
        entity.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")   // ← nome da coluna no banco
                .HasMaxLength(150)
                .IsRequired();

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        // OwnsOne — mapeia o ValueObject CpfValidator
        entity.OwnsOne(u => u.Cpf, cpf =>
        {
            cpf.Property(c => c.Value)
                .HasColumnName("Cpf")     // ← nome da coluna no banco
                .HasMaxLength(14);
        });
    }
}